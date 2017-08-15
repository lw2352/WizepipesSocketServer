using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
[assembly: log4net.Config.XmlConfigurator(Watch = true)]

namespace WizepipesSocketServer
{
    class SocketServer
    {
        //TODO:使用log4net来记录日志。
        public static log4net.ILog Log = log4net.LogManager.GetLogger(typeof(SocketServer));

        public static Hashtable htClient = new Hashtable(); //strAddress--DataItem
        public static Socket ServerSocket;
        public static Hashtable htSendCmd = new Hashtable(); //intID--QueueCmd
        public static CmdItem cmdItem = new CmdItem(); //实例化
        public List<int> AnalyzeList = new List<int>(); //待分析ID的可变长数组

        public string ServerIP = "192.168.3.83";
        public int ServerPort = 8085;
        public int bufferLength = 1009; //每包的长度
        public int Adlength = 1000; // ad数据长度
        public int checkRecDataQueueTimeInterval = 10; // 检查接收数据包队列时间休息间隔(ms)
        public int checkSendDataQueueTimeInterval = 3000; // 检查发送命令队列时间休息间隔(ms)
        public int checkDataBaseQueueTimeInterval = 5000; // 检查数据库命令队列时间休息间隔(ms)

        public int g_totalPackageCount = 600; //600个包
        public bool IsServerOpen = true;
        public int maxTimeOut = 180; //超时未响应时间--3min
        public int maxBadClient = 1; //最大的故障设备数
        public bool IsAutoTest = true;
        public int CapNextTime = 5;

        private ManualResetEvent checkRecDataQueueResetEvent = new ManualResetEvent(true)
            ; //处理接收数据线程；ManualResetEvent:通知一个或多个正在等待的线程已发生事件

        private ManualResetEvent checkSendDataQueueResetEvent = new ManualResetEvent(true)
            ; //处理发送数据线程，把数据哈希表中的数据复制到各个dataItem中的发送队列

        private ManualResetEvent CheckDataBaseQueueResetEvent = new ManualResetEvent(true);

        private const int constCapTimeCmdSendOK = 1;

        private const int constAllCmdSendOK = 2;
        //初始化服务器，给服务参数赋值

        #region get-set访问器（12个）

        public string CfgServerIP
        {
            get { return ServerIP; }
            set { ServerIP = value; }
        }

        public int CfgServerPort
        {
            get { return ServerPort; }
            set { ServerPort = value; }
        }


        public int CfgbufferLength
        {
            get { return bufferLength; }
            set { bufferLength = value; }
        }

        public int CfgAdlength
        {
            get { return Adlength; }
            set { Adlength = value; }
        }

        public int CfgcheckRecDataQueueTimeInterval
        {
            get { return checkRecDataQueueTimeInterval; }
            set { checkRecDataQueueTimeInterval = value; }
        }

        public int CfgcheckSendDataQueueTimeInterval
        {
            get { return checkSendDataQueueTimeInterval; }
            set { checkSendDataQueueTimeInterval = value; }
        }

        public int CfgcheckDataBaseQueueTimeInterval
        {
            get { return checkDataBaseQueueTimeInterval; }
            set { checkDataBaseQueueTimeInterval = value; }
        }

        public int Cfgg_totalPackageCount
        {
            get { return g_totalPackageCount; }
            set { g_totalPackageCount = value; }
        }

        public int CfgmaxTimeOut
        {
            get { return maxTimeOut; }
            set { maxTimeOut = value; }
        }

        public int CfgmaxBadClient
        {
            get { return maxBadClient; }
            set { maxBadClient = value; }
        }

        public int CfgIsAutoTest
        {
            get { return Convert.ToInt32(IsAutoTest); }
            set { IsAutoTest = Convert.ToBoolean(value); }
        }

        public int CfgCapNextTime
        {
            get { return CapNextTime; }
            set { CapNextTime = value; }
        }

        #endregion

        public bool OpenServer()
        {
            try
            {

                ServerSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                IPEndPoint ipEndPoint = new IPEndPoint(IPAddress.Parse(ServerIP), ServerPort);

                //Bind and listen on the given address
                ServerSocket.Bind(ipEndPoint);
                ServerSocket.Listen(100);

                ServerSocket.BeginAccept(new AsyncCallback(OnAccept), null);

                //接收数据包处理线程
                if (!ThreadPool.QueueUserWorkItem(CheckRecDataQueue))
                    return false;
                //发送数据包处理线程
                if (!ThreadPool.QueueUserWorkItem(CheckSendDataQueue))
                    return false;
                if (!ThreadPool.QueueUserWorkItem(CheckDataBaseQueue))
                    return false;

                Log.Debug("服务开启成功");
                return true;
            }
            catch (Exception ex)
            {
                Log.Debug(ex);
                Log.Debug("服务开启失败");
                string error = DateTime.Now.ToString() + "出错信息：" + "---" + ex.Message + "\n";
                System.Diagnostics.Debug.WriteLine(error);
                return false;
            }
        }

        public void CloseServer()
        {
            try
            {
                IsServerOpen = false;
                checkRecDataQueueResetEvent.WaitOne();
                checkSendDataQueueResetEvent.WaitOne();
                foreach (DataItem dataItem in htClient.Values)
                {
                    dataItem.CloseSocket();
                }
                ServerSocket.Close();
                htClient.Clear();
                htSendCmd.Clear();
                Log.Debug("服务关闭成功");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                Log.Debug(ex);
            }

        }


        private void OnAccept(IAsyncResult ar)
        {
            try
            {
                if (IsServerOpen == true)
                {
                    Socket clientSocket = ServerSocket.EndAccept(ar);
                    //Start listening for more clients
                    ServerSocket.BeginAccept(OnAccept, null);

                    string strAddress = clientSocket.RemoteEndPoint.ToString();
                    if (!htClient.ContainsKey(strAddress))
                    {
                        DataItem dataItem = new DataItem();

                        dataItem.Init(clientSocket, bufferLength, strAddress, 0,
                            Adlength * g_totalPackageCount); //初始化dataItem

                        htClient.Add(strAddress, dataItem);

                        Console.WriteLine(DateTime.Now.ToString() + "收到客户端" + strAddress + "的连接请求" + "\n");
                        Log.Debug("收到客户端" + strAddress + "的连接请求" + "\n");
                        //开始从连接的socket异步接收数据
                        dataItem.socket.BeginReceive(dataItem.buffer, 0, dataItem.buffer.Length, SocketFlags.None,
                            OnReceive, clientSocket);
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Debug(ex);
                string error = DateTime.Now.ToString() + "出错信息：" + "---" + ex.Message + "\n";
                System.Diagnostics.Debug.WriteLine(error);
            }
        }

        public void OnReceive(IAsyncResult ar)
        {
            byte[] ID = new byte[4]; //设备的ID号
            int bytesRead = 0;
            string strAddress = null;
            try
            {
                Socket clientSocket = ar.AsyncState as Socket;
                strAddress = clientSocket.RemoteEndPoint.ToString();

                DataItem dataItem = (DataItem) htClient[strAddress]; //取出address对应的dataitem

                bytesRead = clientSocket.EndReceive(ar); //接收到的数据长度

                if (bytesRead == 0)
                {
                    Console.WriteLine("设备断开连接");
                    dataItem.status.clientStage = ClientStage.offLine;
                    dataItem.CloseSocket(); //关闭socket
                }
                else if (dataItem.buffer[0] == 0xA5 && dataItem.buffer[1] == 0xA5 &&
                         dataItem.buffer[bytesRead - 2] == 0x5A && dataItem.buffer[bytesRead - 1] == 0x5A) //判断报文头和尾
                {
                    //test
                    string str = byteToHexStr(dataItem.buffer);
                    string strrec = str.Substring(0, bytesRead * 2);
                    Console.WriteLine(DateTime.Now + "从硬件" + dataItem.strAddress + "设备号--" + dataItem.intDeviceID +
                                      "接收到的数据长度是" + bytesRead.ToString() + "数据是" + strrec + "\n");

                    byte[] recData = new byte[bytesRead];
                    Array.Copy(dataItem.buffer, recData, bytesRead);
                    dataItem.recDataQueue.Enqueue(recData); //Enqueue 将对象添加到 Queue<T> 的结尾处

                    if (dataItem.buffer[2] == 0xFF)
                    {
                        if (dataItem.intDeviceID == 0) //只判断新地址的心跳包，避免重复检测
                        {
                            //设备的ID字符串
                            ID[0] = dataItem.buffer[3];
                            ID[1] = dataItem.buffer[4];
                            ID[2] = dataItem.buffer[5];
                            ID[3] = dataItem.buffer[6];
                            int intdeviceID = byteToInt(ID);

                            string oldAddress = checkIsHaveID(intdeviceID); //得到当前ID对应的旧地址

                            if (oldAddress == null)
                            {
                                //若不存在，属于全新地址，更新ID号
                                dataItem.intDeviceID = intdeviceID;
                                //把信息存入数据库
                                NetDb.addsensorinfo(intdeviceID, dataItem.strAddress, dataItem.strAddress,
                                    dataItem.status.HeartTime.ToString(),
                                    Convert.ToInt32(dataItem.status.clientStage),
                                    Convert.ToInt32(dataItem.status.adStage));
                                Log.Debug("新设备号:" + intdeviceID + "-地址为" + dataItem.strAddress + "-已连接");
                            }
                            else
                            {
                                //若存在，把旧地址的status数据属性复制到新地址上
                                DataItem olddataItem = (DataItem) htClient[oldAddress]; //取出旧的dataitem
                                //更新进哈希表
                                dataItem.intDeviceID = intdeviceID;
                                dataItem.status = olddataItem.status;
                                dataItem.status.clientStage = ClientStage.idle;
                                dataItem.status.HeartTime = DateTime.Now;
                                dataItem.recDataQueue = olddataItem.recDataQueue;
                                dataItem.sendDataQueue = olddataItem.sendDataQueue;

                                //把信息存入数据库
                                NetDb.addsensorinfo(intdeviceID, dataItem.strAddress, dataItem.strAddress,
                                    dataItem.status.HeartTime.ToString(),
                                    Convert.ToInt32(dataItem.status.clientStage),
                                    Convert.ToInt32(dataItem.status.adStage));

                                htClient.Remove(oldAddress); //删除旧地址的键值对
                                Log.Debug("设备号为:" + intdeviceID + "删除旧地址:" + oldAddress + "添加新地址:" +
                                          dataItem.strAddress);
                            }
                        } //if (dataItem.intDeviceID == 0)

                    } // if (dataItem.buffer[2] == 0xFF)
                } //else if 

                clientSocket.BeginReceive(dataItem.buffer, 0, dataItem.buffer.Length, SocketFlags.None, OnReceive,
                    clientSocket);
            }
            catch (ObjectDisposedException ex)
            {
                System.Diagnostics.Debug.WriteLine("Socket 已关闭");
                Log.Debug("Socket 已关闭" + ex);
            }
            catch (SocketException ex)
            {
                System.Diagnostics.Debug.WriteLine("尝试访问套接字时出错");
                Log.Debug("尝试访问套接字时出错" + ex);
            }
            catch (Exception ex)
            {
                //客户端强制断开连接               
                Log.Debug(ex);
                System.Diagnostics.Debug.WriteLine(ex);
            }
        }



        //7-22 处理接收队列
        private void CheckRecDataQueue(object state)
        {
            checkRecDataQueueResetEvent.Reset(); //Reset()将事件状态设置为非终止状态，导致线程阻止。
            while (IsServerOpen)
            {
                try
                {
                    foreach (DataItem dataItem in htClient.Values)
                    {
                        dataItem.HandleData();
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex);
                    Log.Debug(ex);
                }
                Thread.Sleep(checkRecDataQueueTimeInterval); //当前数据处理线程休眠一段时间
            }
            checkRecDataQueueResetEvent.Set();
        }


        //发送队列里面的命令
        private void CheckSendDataQueue(object state)
        {
            checkSendDataQueueResetEvent.Reset(); //Reset()将事件状态设置为非终止状态，导致线程阻止。
            while (IsServerOpen)
            {
                try
                {
                    int adFinishedClientNum = 0; //发送采样完成信息的设备数
                    int offlineClientNum = 0; //已离线设备（超过3分钟没有连上）
                    int adUploadingAndOnlineClinetNum = 0; //AD数据已存储的设备数,也是在线的设备

                    string deleteAddress = null;
                    foreach (DataItem dataItem in htClient.Values)
                    {
                        dataItem.SendData();
                        dataItem.CheckTimeout(maxTimeOut);
                        if (dataItem.status.clientStage == ClientStage.offLine &&
                            dataItem.intDeviceID == 0) //7-28 无效项，需删除
                        {
                            deleteAddress = dataItem.strAddress;
                        }
                        if (dataItem.status.adStage == AdStage.AdFinished)
                        {
                            adFinishedClientNum++;
                        }
                        if (dataItem.status.clientStage == ClientStage.offLine)
                        {
                            offlineClientNum++;
                        }
                        if (dataItem.status.adStage == AdStage.AdUploading &&
                            dataItem.status.clientStage == ClientStage.idle) //在线且没有上传完毕的设备数要为0，一定要等，才能进行下一步
                        {
                            adUploadingAndOnlineClinetNum++;
                        }
                        if (dataItem.status.adStage == AdStage.AdStored)
                        {
                            if (!AnalyzeList.Contains(dataItem.intDeviceID)) //避免重复添加
                            {
                                AnalyzeList.Add(dataItem.intDeviceID); //添加id号
                            }
                            if (dataItem.status.IsGetADNow == false && IsAutoTest == false)
                            {
                                dataItem.status.adStage = AdStage.Idle;
                            }
                        }

                        if (htSendCmd.ContainsKey(dataItem.intDeviceID)) //发送命令哈希表中是否包含当前dataItem的id
                        {
                            Queue<byte[]> htsendCmdQueue = htSendCmd[dataItem.intDeviceID] as Queue<byte[]>;
                            while (htsendCmdQueue != null && htsendCmdQueue.Count > 0)
                            {
                                dataItem.sendDataQueue.Enqueue(htsendCmdQueue.Dequeue()); //复制数据
                            }
                        }
                    } //end of foreach
                    if (deleteAddress != null && htClient.ContainsKey(deleteAddress))
                    {
                        htClient.Remove(deleteAddress);
                        Log.Debug(deleteAddress + "是无效项，从哈希表中删除");
                    }

                    //采集完成，准备上传
                    if (adFinishedClientNum >= (htClient.Count - offlineClientNum - maxBadClient) &&
                        (adFinishedClientNum > maxBadClient))
                    {
                        //开始上传
                        Console.WriteLine("开始上传");
                        UploadData();
                        Log.Debug("开始上传");
                    }
                    //上传完成，准备分析
                    if ((AnalyzeList.Count >= htClient.Count - offlineClientNum - maxBadClient) &&
                        (AnalyzeList.Count > maxBadClient) &&
                        adUploadingAndOnlineClinetNum == 0) //没有正在上传的设备且上传完成的设备数大于等于总数减去容许故障设备数
                    {
                        AnalyzeData(); //分析AD数据并保存结果到数据库

                        if (IsAutoTest == true)
                        {
                            //开始上传
                            Console.WriteLine("AD数据存储完毕，重设时间");
                            SetCapTime(0xFF); //自动测试打开后，采样完成会设置下一次的采样时间
                            Log.Debug("AD数据存储完毕，重设时间");
                        }
                    }

                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex);
                    Log.Debug(ex);
                }
                Thread.Sleep(checkSendDataQueueTimeInterval); //当前数据处理线程休眠一段时间
            }
            checkSendDataQueueResetEvent.Set();
        }

        //读取数据库命令线程
        public void CheckDataBaseQueue(object state)
        {
            int[] cfg = new int[7]; //存储从数据库读取的设备配置参数
            //收到设备数据后写数据库，表示有回复（发送成功），不再重复发送
            while (IsServerOpen)
            {
                try
                {
                    foreach (DataItem dataItem in htClient.Values)
                    {
                        cfg = NetDb.readsensorcfg(dataItem
                            .intDeviceID); //从数据库读取设备的配置参数                                              

                        if (cfg != null && cfg[0] == 0)
                        {
                            byte[] cmdCapTime = cmdItem.CmdSetCapTime;
                            cmdCapTime[9] = (byte) cfg[1];
                            cmdCapTime[10] = (byte) cfg[2];

                            byte[] cmdSetOpenAndCloseTime = cmdItem.CmdSetOpenAndCloseTime;
                            cmdSetOpenAndCloseTime[9] = (byte) (2 * cfg[3] >> 8);
                            cmdSetOpenAndCloseTime[10] = (byte) (2 * cfg[3] & 0xFF);
                            cmdSetOpenAndCloseTime[11] = (byte) (2 * cfg[4] >> 8);
                            cmdSetOpenAndCloseTime[12] = (byte) (2 * cfg[4] & 0xFF);

                            Queue<byte[]> DbCmdQueue = new Queue<byte[]>();
                            DbCmdQueue.Enqueue(cmdCapTime);
                            DbCmdQueue.Enqueue(cmdSetOpenAndCloseTime);
                            if (!htSendCmd.ContainsKey(dataItem.intDeviceID)) //不存在ID则添加
                            {
                                htSendCmd.Add(dataItem.intDeviceID, DbCmdQueue);
                                string msg = "设备号:" + dataItem.intDeviceID + "不存在ID则添加,htSendCmd添加命令队列成功";
                                Console.WriteLine(msg);
                                Log.Debug(msg);
                            }
                            else //存在则更新
                            {
                                htSendCmd[dataItem.intDeviceID] = DbCmdQueue;
                                string msg = "设备号: " + dataItem.intDeviceID + "存在则更新,htSendCmd添加命令队列成功";
                                Console.WriteLine(msg);
                                Log.Debug(msg);
                            }

                        }

                        //获取设备的GPS经纬度
                        if (cfg != null && cfg[6] == 1 && cfg[0] == constAllCmdSendOK)
                        {
                            AddCmdToQueue(dataItem.intDeviceID, cmdItem.CmdReadGPSData);
                        }

                        //立即采样（加3分钟）流程
                        if (cfg != null && cfg[5] == 1 && cfg[0] == constAllCmdSendOK)
                        {
                            dataItem.status.IsGetADNow = true;
                            SetCapTime(dataItem.intDeviceID);
                        }
                        else if (cfg != null && cfg[5] == 1 && cfg[0] == constCapTimeCmdSendOK)
                        {
                            //立即采样时间设置成功,把标志位复位
                            NetDb.addsensorcfg(dataItem.intDeviceID, 2, cfg[1], cfg[2], cfg[3], cfg[4], 0);
                            Log.Debug("立即采样时间设置成功,把标志位复位");
                        }

                        if (dataItem.status.IsGetADNow == true && dataItem.status.adStage == AdStage.AdStored)
                        {
                            //立即采样完成后，重新设置一次采样时刻
                            NetDb.addsensorcfg(dataItem.intDeviceID, 0, cfg[1], cfg[2], cfg[3], cfg[4], 0);
                            dataItem.status.IsGetADNow = false;
                            dataItem.status.adStage = AdStage.Idle;
                            Log.Debug("立即采样完成后，重新设置一次采样时刻");
                            //adstage的状态变化太快的话，前台不能从数据库检测到
                            //NetDb.addsensorinfo(dataItem.intDeviceID, dataItem.strAddress, dataItem.strAddress,
                            //dataItem.status.HeartTime.ToString(),
                            //Convert.ToInt32(dataItem.status.clientStage), Convert.ToInt32(dataItem.status.adStage));
                        }

                    } //end of foreach
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    Log.Debug(ex);
                }
                Thread.Sleep(checkDataBaseQueueTimeInterval);
            }
            CheckDataBaseQueueResetEvent.Set();
        }

        //分析AnalyzeList中的数据
        private void AnalyzeData()
        {
            List<string> resultList = new List<string>(); //计算结果列表
            //对所有上传完成的设备进行基点分析，把结果写入数据库
            for (int i = 0; i < AnalyzeList.Count - 1; i++) //at least 2 device
            {
                int idA = AnalyzeList[i];
                for (int j = i + 1; j < AnalyzeList.Count; j++)
                {
                    int idB = AnalyzeList[j];
                    resultList = Net_Analyze.AutoAnalyze(idA, idB);
                    Net_Analyze_DB.writeAnalyzeResult(idA, idB, resultList[0], DateTime.Now.ToString(), 0,
                        resultList[1], resultList[2], resultList[3]);
                    Log.Debug("设备" + idA + "号和" + idB + "号的基点为：" + resultList[0]);
                    Log.Debug("图片路径分别为：" + resultList[1]+ resultList[2]+ resultList[3]);
                    Console.WriteLine("设备" + idA + "号和" + idB + "号的基点为：" + resultList[0]);
                    Console.WriteLine("图片路径分别为：" + resultList[1] + resultList[2] + resultList[3]);
                }
            }
            AnalyzeList.Clear();
            Console.WriteLine("分析完成,清空list");
        }

        //检查哈希表中是否已存在当前ID
        public string checkIsHaveID(int id)
        {
            foreach (DataItem dataItem in htClient.Values)
            {
                if (dataItem.intDeviceID == id) //设备掉线后address(is key)改变而ID号不变
                    return dataItem.strAddress;
            }
            return null;
        }

        // 字节数组转16进制字符串 
        private static string byteToHexStr(byte[] bytes)
        {
            string returnStr = "";
            if (bytes != null)
            {
                for (long i = 0; i < bytes.Length; i++)
                {
                    returnStr += bytes[i].ToString("X2");
                }
            }
            return returnStr;
        }

        //字节数组转int值
        public static int byteToInt(byte[] bytes)
        {
            int returnInt = 0;
            if (bytes != null)
            {
                for (long i = 0; i < bytes.Length; i++)
                {
                    if (i == 3)
                        returnInt += (int) bytes[i];
                    else if (i == 2)
                        returnInt += (int) bytes[i] * 256;
                    else if (i == 1)
                        returnInt += (int) bytes[i] * 65536;
                    else if (i == 0)
                        returnInt += (int) bytes[i] * 16777216;
                }
            }
            return returnInt;
        }


        public void UploadData()
        {
            byte[] CmdAD = cmdItem.CmdADPacket;
            foreach (DataItem dataItem in htClient.Values)
            {
                dataItem.status.IsSendDataToServer = true;
                dataItem.status.currentsendbulk = 0;
                dataItem.status.datalength = 0;
                dataItem.status.adStage = AdStage.AdUploading;
                NetDb.addsensorinfo(dataItem.intDeviceID, dataItem.strAddress, dataItem.strAddress,
                    dataItem.status.HeartTime.ToString(),
                    Convert.ToInt32(dataItem.status.clientStage), Convert.ToInt32(dataItem.status.adStage));

                dataItem.sendDataQueue.Enqueue(CmdAD);
            }
        }

        public void SetCapTime(int id)
        {
            byte[] cmd = cmdItem.CmdSetCapTime;
            //int NextTime = Convert.ToInt32(nextTime);
            if (DateTime.Now.Minute + CapNextTime <= 59)
            {
                cmd[9] = (byte) DateTime.Now.Hour;
                cmd[10] = (byte) (DateTime.Now.Minute + CapNextTime); //当前时刻加5分钟
            }
            else
            {
                //分钟数大于60
                cmd[9] = (byte) (DateTime.Now.Hour + 1);
                cmd[10] = (byte) (DateTime.Now.Minute + CapNextTime - 60 + 1);
            }

            if (IsAutoTest == true)
            {
                AddCmdToQueue(0xFF, cmd);
            }
            else AddCmdToQueue(id, cmd);
        }

        //for set capTime
        public void AddCmdToQueue(int id, byte[] cmd)
        {
            if (id == 0xFF) //立即采样的测试使用
            {
                foreach (DataItem dataItem in htClient.Values)
                {
                    dataItem.sendDataQueue.Enqueue(cmd);
                    dataItem.status.adStage = AdStage.Idle;
                }
            }
            else
            {
                foreach (DataItem dataItem in htClient.Values)
                {
                    if (dataItem.intDeviceID == id)
                    {
                        dataItem.sendDataQueue.Enqueue(cmd);
                        if (cmd[2] == 0x25)
                        {
                            dataItem.status.adStage = AdStage.Idle;
                        }
                    }
                }
            }
        }

        //显示哈希表中设备的各项信息
        public string ViewClientInfo()
        {
            string msg = null;
            foreach (DataItem dataItem in htClient.Values)
            {
                msg += "地址:" + dataItem.strAddress + "-ID号:" + dataItem.intDeviceID + "-包数:" +
                       dataItem.status.currentsendbulk +
                       "-心跳时间:" + dataItem.status.HeartTime.ToLongTimeString() + "-设备状态:" +
                       dataItem.status.clientStage + "-AD状态:" + dataItem.status.adStage + ";\r\n";
            }
            return msg;
        }

        //测试画图的稳定性
        public void TestZed()
        {
            AnalyzeList.Add(3);
            AnalyzeList.Add(4);
            AnalyzeList.Add(5);
            AnalyzeData();
        }
    }

}

