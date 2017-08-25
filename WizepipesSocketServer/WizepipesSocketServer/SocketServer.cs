using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using ZedGraph;

//[assembly: log4net.Config.XmlConfigurator(Watch = true)]

//TODO:1.自动测试的方法要改规范

namespace WizepipesSocketServer
{
    class SocketServer
    {
        public static log4net.ILog Log = log4net.LogManager.GetLogger(typeof(SocketServer));

        public static Hashtable htClient = new Hashtable(); //strAddress--DataItem
        public static Socket ServerSocket;
        public static Hashtable htSendCmd = new Hashtable(); //intID--QueueCmd
        public static int[,] MultiUserList;//多用户立即采样,int[] [userID，aID,bID]//用二维数组，没必要用List
        //对于区域来说，和多用户立即采样一样，判断时使用for循环
        public static List<int[]> AreaDeviceList;//1个数组包含一个区域areaID里面的所有设备ID
        public static List<List<int>> AnalyzeAreaList;//存储已上传完成的设备ID
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

        //处理接收数据线程；ManualResetEvent:通知一个或多个正在等待的线程已发生事件
        private ManualResetEvent checkRecDataQueueResetEvent = new ManualResetEvent(true);

        //处理发送数据线程，把数据哈希表中的数据复制到各个dataItem中的发送队列
        private ManualResetEvent checkSendDataQueueResetEvent = new ManualResetEvent(true);

        private ManualResetEvent CheckDataBaseQueueResetEvent = new ManualResetEvent(true);
        private delegate void AsyncAnalyzeCaptureNowData(int idA, int idB);


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
                CheckDataBaseQueueResetEvent.WaitOne();
                foreach (DataItem dataItem in htClient.Values)
                {
                    dataItem.CloseSocket();
                }
                ServerSocket.Close();
                htClient.Clear();
                htSendCmd.Clear();

                GC.SuppressFinalize(this);
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

                        lock (htClient)
                        {
                            htClient.Add(strAddress, dataItem);
                        }
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

                DataItem dataItem = (DataItem)htClient[strAddress]; //取出address对应的dataitem

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
                    /*string str = byteToHexStr(dataItem.buffer);
                    string strrec = str.Substring(0, bytesRead * 2);
                    Console.WriteLine(DateTime.Now + "从硬件" + dataItem.strAddress + "设备号--" + dataItem.intDeviceID +
                                      "接收到的数据长度是" + bytesRead.ToString() + "数据是" + strrec + "\n");*/

                    if (bytesRead <= bufferLength)
                    {
                        byte[] recData = new byte[bytesRead];
                        Array.Copy(dataItem.buffer, recData, bytesRead);
                        dataItem.recDataQueue.Enqueue(recData); //Enqueue 将对象添加到 Queue<T> 的结尾处
                    }

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
                                NetDb.UpdateSensorCfg(dataItem.intDeviceID, "IsGetGpsInfo", 1);

                                Log.Debug("新设备号:" + intdeviceID + "-地址为" + dataItem.strAddress + "-已连接");
                            }
                            else
                            {
                                //若存在，把旧地址的status数据属性复制到新地址上
                                DataItem olddataItem = (DataItem)htClient[oldAddress]; //取出旧的dataitem
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
                                NetDb.UpdateSensorCfg(dataItem.intDeviceID, "IsGetGpsInfo", 1);

                                lock (htClient)
                                {
                                    htClient.Remove(oldAddress); //删除旧地址的键值对
                                }
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
                        if (dataItem.status.clientStage == ClientStage.offLine && dataItem.intDeviceID == 0) //7-28 无效项，需删除
                        {
                            deleteAddress = dataItem.strAddress;
                        }

                        #region past

                        //对采样完成和上传的判断按照区域来，每个区域的状态要有判断
                        //对立即采样的设备单独处理
                        /*if (dataItem.status.adStage == AdStage.AdFinished && dataItem.status.IsCaptureNow == false)
                        {
                            adFinishedClientNum++;
                        }
                        if (dataItem.status.clientStage == ClientStage.offLine && dataItem.status.IsCaptureNow == false)
                        {
                            offlineClientNum++;
                        }
                        if (dataItem.status.adStage == AdStage.AdUploading && dataItem.status.clientStage == ClientStage.idle && dataItem.status.IsCaptureNow == false) //在线且没有上传完毕的设备数要为0，一定要等，才能进行下一步
                        {
                            adUploadingAndOnlineClinetNum++;
                        }
                        if (dataItem.status.adStage == AdStage.AdStored)
                        {
                            if (!AnalyzeList.Contains(dataItem.intDeviceID)) //避免重复添加
                            {
                                AnalyzeList.Add(dataItem.intDeviceID); //添加id号
                            }
                            if (dataItem.status.IsCaptureNow == false)
                            {
                                dataItem.status.adStage = AdStage.Idle;
                            }
                            if (dataItem.status.IsCaptureNow == true)
                            {
                                dataItem.status.IsCaptureNow = false;
                            }
                        }*/
                        #endregion


                        if (htSendCmd.ContainsKey(dataItem.intDeviceID)) //发送命令哈希表中是否包含当前dataItem的id
                        {
                            //sendCmdQueue应该只读取但不移除，只有当命令发送成功即数据库的IsSet字段为0时才移除
                            List<byte[]> sendCmdQueue = htSendCmd[dataItem.intDeviceID] as List<byte[]>;
                            if (sendCmdQueue != null)
                            {
                                for (int i = 0; i < sendCmdQueue.Count; i++)
                                {
                                    if (sendCmdQueue[i] != null)
                                        dataItem.sendDataQueue.Enqueue(sendCmdQueue[i]); //复制数据
                                }
                            }
                        }
                    } //end of foreach

                    if (deleteAddress != null && htClient.ContainsKey(deleteAddress))
                    {
                        lock (htClient)
                        {
                            htClient.Remove(deleteAddress);
                        }
                        Log.Debug(deleteAddress + "是无效项，从哈希表中删除");
                    }

                    #region past
                    //采集完成，准备上传
                    /*if (adFinishedClientNum >= (htClient.Count - offlineClientNum - maxBadClient) && (adFinishedClientNum > maxBadClient))
                    {
                        //开始上传
                        Console.WriteLine("开始上传");
                        UploadData();
                        Log.Debug("开始上传");
                    }
                    //上传完成，准备分析
                    if ((AnalyzeList.Count >= htClient.Count - offlineClientNum - maxBadClient) && (AnalyzeList.Count > maxBadClient) && adUploadingAndOnlineClinetNum == 0) //没有正在上传的设备且上传完成的设备数大于等于总数减去容许故障设备数
                    {
                        AnalyzeData(AnalyzeList); //分析AD数据并保存结果到数据库

                        if (IsAutoTest == true)
                        {
                            //开始上传
                            Console.WriteLine("AD数据存储完毕，重设时间");
                            SetCapTime(0xFF); //自动测试打开后，采样完成会设置下一次的采样时间
                            Log.Debug("AD数据存储完毕，重设时间");
                        }
                    }*/
                    #endregion

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
            List<int> cfgList = new List<int>(); //存储从数据库读取的设备配置参数
            //收到设备数据后写数据库，表示有回复（发送成功），不再重复发送
            while (IsServerOpen)
            {
                try
                {
                    foreach (DataItem dataItem in htClient.Values)
                    {
                        cfgList = NetDb.readsensorcfg(dataItem.intDeviceID); //从数据库读取设备的配置参数                                              

                        if (cfgList != null)
                        {
                            List<byte[]> DbCmdLsit = new List<byte[]>();

                            //利用索引insert和removeAt指定位置的元素
                            if (htSendCmd.ContainsKey(dataItem.intDeviceID)) //存在则更新
                            {
                                DbCmdLsit = htSendCmd[dataItem.intDeviceID] as List<byte[]>;
                                //string msg = DateTime.Now + "设备号: " + dataItem.intDeviceID + "存在";
                                //Console.WriteLine(msg);
                                //Log.Debug(msg);
                            }
                            else
                            {
                                for (int i = 0; i < cfgList.Count; i++)
                                {
                                    DbCmdLsit.Add(null);
                                }
                            }

                            //Convert.ToInt32(null) == 0;
                            if (cfgList[0] == 1) //设置采样时刻--0x25
                            {
                                byte[] cmdCapTime = cmdItem.CmdSetCapTime;
                                string strHour = NetDb.readsensorcfgItem(dataItem.intDeviceID, "CapTimeHour");
                                string strMinute = NetDb.readsensorcfgItem(dataItem.intDeviceID, "CapTimeMinute");

                                //最多24组
                                string[] HourArray = strHour.Split(new char[] { ',' });
                                string[] MinuteArray = strMinute.Split(new char[] { ',' });

                                List<int> timesList = new List<int>(24);

                                for (int i = 0; i < HourArray.GetLength(0); i++)
                                {
                                    timesList.Add(Convert.ToInt32(HourArray[i]) * 60 + Convert.ToInt32(MinuteArray[i]));
                                }

                                timesList.Sort();//默认从小到大排序,排序完了之后分解并存放在数组中

                                for (int i = 0, j = 9; i < 24; i++)
                                {
                                    if (i < HourArray.GetLength(0))
                                    {
                                        int hour = timesList[i] / 60;
                                        int minute = timesList[i] - 60 * hour;
                                        cmdCapTime[j++] = Convert.ToByte(hour);
                                        cmdCapTime[j++] = Convert.ToByte(minute);
                                    }
                                }
                                //if not include, add it
                                if (!DbCmdLsit.Contains(cmdCapTime))
                                {
                                    DbCmdLsit.RemoveAt(0);
                                    DbCmdLsit.Insert(0, cmdCapTime);
                                    Console.WriteLine("向设备号" + dataItem.intDeviceID + "--加入的命令是:" + byteToHexStr(cmdCapTime));
                                }
                            }
                            else
                            {
                                DbCmdLsit.RemoveAt(0);
                                DbCmdLsit.Insert(0, null);
                            }


                            if (cfgList[1] == 1)//设置开启和关闭时长--0x26
                            {
                                int OpenTime = 2 * (Convert.ToInt32(NetDb.readsensorcfgItem(dataItem.intDeviceID, "OpenTime")));
                                int CloseTime = 2 * (Convert.ToInt32(NetDb.readsensorcfgItem(dataItem.intDeviceID, "CloseTime")));
                                byte[] cmdSetOpenAndCloseTime = cmdItem.CmdSetOpenAndCloseTime;
                                cmdSetOpenAndCloseTime[9] = (byte)(OpenTime >> 8);
                                cmdSetOpenAndCloseTime[10] = (byte)(OpenTime & 0xFF);
                                cmdSetOpenAndCloseTime[11] = (byte)(CloseTime >> 8);
                                cmdSetOpenAndCloseTime[12] = (byte)(CloseTime & 0xFF);

                                if (!DbCmdLsit.Contains(cmdSetOpenAndCloseTime))
                                {
                                    DbCmdLsit.RemoveAt(1);
                                    DbCmdLsit.Insert(1, cmdSetOpenAndCloseTime);
                                    Console.WriteLine("向设备号" + dataItem.intDeviceID + "--加入的命令是:" + byteToHexStr(cmdSetOpenAndCloseTime));
                                }
                            }
                            else
                            {
                                DbCmdLsit.RemoveAt(1);
                                DbCmdLsit.Insert(1, null);
                            }


                            if (cfgList[2] == 1)//是否立即采样--0x28
                            {
                                dataItem.status.IsCaptureNow = true;
                                dataItem.status.adStage = AdStage.Idle;
                                if (DbCmdLsit[2] == null)
                                {
                                    byte[] Cmd = SetCapTime(dataItem.intDeviceID);
                                    DbCmdLsit.RemoveAt(2);
                                    DbCmdLsit.Insert(2, Cmd);
                                    Console.WriteLine("向设备号" + dataItem.intDeviceID + "--加入的命令是:" + byteToHexStr(Cmd));
                                }

                            }
                            else
                            {
                                DbCmdLsit.RemoveAt(2);
                                DbCmdLsit.Insert(2, null);
                            }


                            if (cfgList[3] == 1)//是否读取Gps经纬度信息--0x27
                            {
                                if (!DbCmdLsit.Contains(cmdItem.CmdReadGPSData))
                                {
                                    DbCmdLsit.RemoveAt(3);
                                    DbCmdLsit.Insert(3, cmdItem.CmdReadGPSData);
                                    Console.WriteLine("向设备号" + dataItem.intDeviceID + "--加入的命令是:" + byteToHexStr(cmdItem.CmdReadGPSData));
                                }
                            }
                            else
                            {
                                DbCmdLsit.RemoveAt(3);
                                DbCmdLsit.Insert(3, null);
                            }

                            if (cfgList[4] == 1)//是否设置AP名称--0x31
                            {
                                byte[] Cmd = cmdItem.CmdSetAPssid;
                                byte[] SetAPName = strToByte(NetDb.readsensorcfgItem(dataItem.intDeviceID, "ApName"));//转换成字符型
                                for (int i = 0, j = 9; i < SetAPName.Length; i++)
                                {
                                    Cmd[j++] = SetAPName[i];
                                }
                                if (!DbCmdLsit.Contains(Cmd))
                                {
                                    DbCmdLsit.RemoveAt(4);
                                    DbCmdLsit.Insert(4, Cmd);
                                    Console.WriteLine("向设备号" + dataItem.intDeviceID + "--加入的命令是:" + byteToHexStr(Cmd));
                                }

                            }
                            else
                            {
                                DbCmdLsit.RemoveAt(4);
                                DbCmdLsit.Insert(4, null);
                            }

                            if (cfgList[5] == 1)//是否设置AP密码--0x32
                            {
                                byte[] Cmd = cmdItem.CmdSetAPpassword;
                                byte[] SetAPpassword = strToByte(NetDb.readsensorcfgItem(dataItem.intDeviceID, "ApPassword"));
                                for (int i = 0, j = 9; i < SetAPpassword.Length; i++)
                                {
                                    Cmd[j++] = SetAPpassword[i];
                                }
                                if (!DbCmdLsit.Contains(Cmd))
                                {
                                    DbCmdLsit.RemoveAt(5);
                                    DbCmdLsit.Insert(5, Cmd);
                                    Console.WriteLine("向设备号" + dataItem.intDeviceID + "--加入的命令是:" + byteToHexStr(Cmd));
                                }

                            }
                            else
                            {
                                DbCmdLsit.RemoveAt(5);
                                DbCmdLsit.Insert(5, null);
                            }

                            if (cfgList[6] == 1)//是否设置Server的IP--0x29
                            {
                                byte[] Cmd = cmdItem.CmdSetServerIP;
                                string ipAddress = NetDb.readsensorcfgItem(dataItem.intDeviceID, "ServerIP");
                                string[] sArray = ipAddress.Split(new char[] { '.' });

                                Cmd[9] = Convert.ToByte(sArray[0]);
                                Cmd[10] = Convert.ToByte(sArray[1]);
                                Cmd[11] = Convert.ToByte(sArray[2]);
                                Cmd[12] = Convert.ToByte(sArray[3]);
                                if (!DbCmdLsit.Contains(Cmd))
                                {
                                    DbCmdLsit.RemoveAt(6);
                                    DbCmdLsit.Insert(6, Cmd);
                                    Console.WriteLine("向设备号" + dataItem.intDeviceID + "--加入的命令是:" + byteToHexStr(Cmd));
                                }

                            }
                            else
                            {
                                DbCmdLsit.RemoveAt(6);
                                DbCmdLsit.Insert(6, null);
                            }

                            if (cfgList[7] == 1)//是否设置Server的Port--0x30
                            {
                                byte[] Cmd = cmdItem.CmdSetServerPort;
                                byte[] bytePort = new byte[2];

                                int port = Convert.ToInt32(NetDb.readsensorcfgItem(dataItem.intDeviceID, "ServerPort"));
                                bytePort = intToBytes(port);
                                Cmd[9] = bytePort[0];
                                Cmd[10] = bytePort[1];
                                if (!DbCmdLsit.Contains(Cmd))
                                {
                                    DbCmdLsit.RemoveAt(7);
                                    DbCmdLsit.Insert(7, Cmd);
                                    Console.WriteLine("向设备号" + dataItem.intDeviceID + "--加入的命令是:" + byteToHexStr(Cmd));
                                }
                            }
                            else
                            {
                                DbCmdLsit.RemoveAt(7);
                                DbCmdLsit.Insert(7, null);
                            }

                            if (cfgList[8] == 1) //设备重连--0x33
                            {
                                byte[] CmdReconnectTcp = cmdItem.CmdReconnectTcp;
                                if (!DbCmdLsit.Contains(CmdReconnectTcp))
                                {
                                    DbCmdLsit.RemoveAt(8);
                                    DbCmdLsit.Insert(8, CmdReconnectTcp);
                                    NetDb.UpdateSensorCfg(dataItem.intDeviceID, "IsReconnect", 0);
                                    NetDb.UpdateSensorInfo(dataItem.intDeviceID, "Status", 0);
                                    Console.WriteLine("向设备号" + dataItem.intDeviceID + "--加入的命令是:" + byteToHexStr(CmdReconnectTcp));
                                }

                            }
                            else
                            {
                                DbCmdLsit.RemoveAt(8);
                                DbCmdLsit.Insert(8, null);
                            }

                            if (htSendCmd.ContainsKey(dataItem.intDeviceID)) //存在则更新
                            {
                                htSendCmd[dataItem.intDeviceID] = DbCmdLsit;
                                //string msg = DateTime.Now + "设备号: " + dataItem.intDeviceID + "存在则更新";
                                //Console.WriteLine(msg);
                                //Log.Debug(msg);
                            }

                            //把从数据库读取的命令添加到队列中
                            if (!htSendCmd.ContainsKey(dataItem.intDeviceID) && DbCmdLsit.Count > 0) //不存在ID则添加
                            {
                                htSendCmd.Add(dataItem.intDeviceID, DbCmdLsit);
                                string msg = DateTime.Now + "设备号:" + dataItem.intDeviceID + "不存在ID则添加,htSendCmd添加命令队列成功";
                                Console.WriteLine(msg);
                                Log.Debug(msg);
                            }


                        }

                    } //end of foreach

                    //读取数据库的立即采样设备对
                    MultiUserList = NetDb.GetDevicePair();//"userID"]);"SensorAID"]);"SensorBID"
                    List<int> captureNowOverIDList = new List<int>();
                    if (MultiUserList != null)
                    {
                        int[,] checkResult = new int[MultiUserList.GetLength(0),
                            MultiUserList.GetLength(1) - 1]; //userID--0/1;其中1表示两个设备都上传完成，0表示未结束 
                        checkResult = checkDevicePair(MultiUserList); //得到对比结果<userID, 0/1>!!!
                        for (int i = 0; i < checkResult.GetLength(0); i++)
                        {
                            //把结果写入数据库
                            if (checkResult[i, 1] == 1)
                            {
                                NetDb.UpdateMultiUser("IsCapture", checkResult[i, 0], 0); //写入数据库表示立即采样完成
                                //复位设备的立即采样属性
                                captureNowOverIDList.Add(MultiUserList[i, 1]);
                                captureNowOverIDList.Add(MultiUserList[i, 2]);
                            }
                        }
                    }

                    if (AreaDeviceList == null)
                    {
                        //初始化读取areaID里面的设备ID
                        AreaDeviceList = NetDb.readAllDeviceIdByAreaID();
                    }
                    if (AnalyzeAreaList == null && AreaDeviceList != null)
                    {
                        //初始化
                        AnalyzeAreaList = new List<List<int>>(AreaDeviceList.Count);//主要是初始化行数
                        List<int> tempList = new List<int>();
                        for (int i = 0; i < AreaDeviceList.Count; i++)
                        {
                            AnalyzeAreaList.Add(tempList);
                        }
                    }
                    int[,] checkAreaResult = new int[AreaDeviceList.Count, 3];
                    foreach (DataItem dataItem in htClient.Values)
                    {


                        for (int i = 0; i < AreaDeviceList.Count; i++)
                        {
                            if (AreaDeviceList[i].Contains(dataItem.intDeviceID))
                            {
                                //TODO：对设备具体的状态属性判断，并记录下来;每个区域的采样完成和离线的总数目记录在一个二维数组中，当此区域达到上传的资格后，进行上传
                                if (dataItem.status.adStage == AdStage.AdFinished && dataItem.status.IsCaptureNow == false)
                                {
                                    checkAreaResult[i, 0]++;//采样完成设备数
                                }
                                if (dataItem.status.clientStage == ClientStage.offLine && dataItem.status.IsCaptureNow == false)
                                {
                                    checkAreaResult[i, 1]++;//离线设备数
                                }
                                if (dataItem.status.adStage == AdStage.AdUploading && dataItem.status.clientStage == ClientStage.idle && dataItem.status.IsCaptureNow == false)
                                {
                                    checkAreaResult[i, 2]++;//在线且正在上传设备数

                                }
                                if (dataItem.status.adStage == AdStage.AdStored && dataItem.status.IsCaptureNow == false)
                                {
                                    //上传完成设备数每次都在变化，只需要把完成的设备ID存起来，后面直接读取count
                                    AnalyzeAreaList[i].Add(dataItem.intDeviceID);//把上传完成的设备加入list中
                                    //复位idle
                                    dataItem.status.adStage = AdStage.Idle;
                                }

                            }// end of if contains
                        }// end of for
                    }//end of foreach

                    //for循环判断每一组的情况(计算比较checkAreaResult)
                    //上传
                    //分析
                    for (int i = 0; i < AreaDeviceList.Count; i++)
                    {
                        //采集完成，准备上传
                        if ((checkAreaResult[i, 0] >= (htClient.Count - checkAreaResult[i, 1] - maxBadClient)) && (checkAreaResult[i, 0] > maxBadClient))
                        {
                            //开始上传
                            Console.WriteLine("开始上传");
                            UploadData();
                            Log.Debug("开始上传");
                        }
                        //上传完成，准备分析
                        if ((AnalyzeAreaList.Count > 0) && (AnalyzeAreaList[i].Count >= AreaDeviceList[i].Length - checkAreaResult[i, 1] - maxBadClient) && (AnalyzeAreaList[i].Count > maxBadClient) && (checkAreaResult[i, 2] == 0)) //没有正在上传的设备且上传完成的设备数大于等于总数减去容许故障设备数
                        {
                            AnalyzeData(AnalyzeAreaList[i]); //分析AD数据并保存结果到数据库

                            if (IsAutoTest == true)
                            {
                                //TODO:把所有设备的立即采样属性设成立即采样
                                //根据两个传感器在同一个管道来设置
                            }
                        }
                    }


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
        private void AnalyzeData(List<int> analyzeList)
        {

            //对所有上传完成的设备进行基点分析，把结果写入数据库
            for (int i = 0; i < AnalyzeList.Count - 1; i++) //at least 2 device
            {
                int idA = AnalyzeList[i];
                for (int j = i + 1; j < analyzeList.Count; j++)
                {
                    int idB = AnalyzeList[j];
                    AnalyzeCaptureNowData(idA, idB);

                }
            }
            AnalyzeList.Clear();
            Console.WriteLine("分析完成,清空list");
        }

        /// <summary>
        /// 分析两个设备的立即采样数据
        /// </summary>
        /// <param name="idA"></param>
        /// <param name="idB"></param>
        public void AnalyzeCaptureNowData(int idA, int idB)
        {
            //TODO:具体的业务操作
            List<string> resultList = new List<string>(); //计算结果列表
            List<int> pipeInfoList = NetDb.GetpipeInfo(idA, idB);//包括：sensorAID);sensorBID);pipeID);pipeLength);
            if (pipeInfoList != null && pipeInfoList[0] != 0 && pipeInfoList[1] != 0 && pipeInfoList[2] != 0)
            {
                string distance = "";
                string[] sensorName = new string[2];
                sensorName = NetDb.GetSensorNameAndID(idA);//TODO:读取idA设备对应的地图SensorID和Name
                if (pipeInfoList[3] != 0)//读出了管子长度
                {
                    resultList = Net_Analyze.AutoAnalyze(idA, idB);

                    distance = (CalculateOffset(Convert.ToInt32(resultList[0]), pipeInfoList[3], 1000, 5000)).ToString();
                }
                else distance = "fail";

                Net_Analyze_DB.writeAnalyzeResult(idA, idB, resultList[0], DateTime.Now.ToString(), pipeInfoList[2],
                    resultList[1], resultList[2], resultList[3], sensorName[1], distance);
                Log.Debug("设备" + idA + "号和" + idB + "号的基点为：" + resultList[0]);
                Log.Debug("图片路径分别为：" + resultList[1] + resultList[2] + resultList[3]);
                Console.WriteLine("设备" + idA + "号和" + idB + "号的基点为：" + resultList[0]);
                Console.WriteLine("图片路径分别为：" + resultList[1] + resultList[2] + resultList[3]);
                Console.WriteLine("管道信息为：" + "管子ID：" + pipeInfoList[2] + "--SensorName：" + sensorName[1] + "-距离是：" + distance);

                if (Convert.ToDouble(distance) > 0 && Convert.ToDouble(distance) < pipeInfoList[3])
                {
                    NetDb.UpdateLeakTimes(pipeInfoList[2]);//更新管道漏水次数

                    double scale = 0;
                    if (NetDb.getSensorIsPipeStart(pipeInfoList[2].ToString(), sensorName[0]) == "1")//标记漏点在管子上的比例位置：1-尾部，0-头部
                    {
                        scale = 1 - (Convert.ToDouble(distance) / pipeInfoList[3]);
                    }
                    else scale = Convert.ToDouble(distance) / pipeInfoList[3]; 

                    NetDb.UpdateLeakPointScale(pipeInfoList[2], scale);
                }
            }
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

        public int[,] checkDevicePair(int[,] devicePair)
        {
            try
            {
                int[,] result = new int[devicePair.GetLength(0), devicePair.GetLength(1) - 1]; //二维(N行*2列)数组 <userID--0/1>
                int[,] resultPair = new int[devicePair.GetLength(0), devicePair.GetLength(1) - 1]; //<0/1, 0/1>
                foreach (DataItem dataItem in htClient.Values) //总表
                {
                    for (int i = 0; i < resultPair.GetLength(0); i++) //行
                    {
                        for (int j = 0; j < resultPair.GetLength(1); j++) //列
                        {
                            if (dataItem.intDeviceID == devicePair[i, j+1])//循环匹配ID号
                            {
                                if (dataItem.status.adStage == AdStage.AdStored)
                                {
                                    resultPair[i, j] = 1;

                                }
                                else
                                {
                                    resultPair[i, j] = 0;
                                }
                            }
                        }

                    }
                } //end of foreach

                for (int i = 0; i < result.GetLength(0); i++)
                {
                    result[i, 0] = devicePair[i, 0];
                    if (resultPair[i, 0] == 1 && resultPair[i, 1] == 1)
                    {
                        result[i, 1] = 1;
                        AsyncAnalyzeCaptureNowData method = new AsyncAnalyzeCaptureNowData(AnalyzeCaptureNowData);
                        method.BeginInvoke(devicePair[i, 1], devicePair[i, 2], null, null);
                    }
                    else result[i, 1] = 0;
                }

                return result;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }


        /*
       *根据参数计算偏移值
       *偏移点数：n    正数A超前B，负数A滞后B
       *管道长度：l  
       *传播速率：v    1000m/s
       *采样频率：f    5000Hz
       *计算公式：x=1/2*(l-n*v/f)
       */
        public double CalculateOffset(double n, double l, double v, double f)
        {
            double x = (0.5 * (l - n * v / f));
            //保留3位小数
            x = (int)(x * 1000);
            x = x / 1000;

            return x;
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
                        returnInt += (int)bytes[i];
                    else if (i == 2)
                        returnInt += (int)bytes[i] * 256;
                    else if (i == 1)
                        returnInt += (int)bytes[i] * 65536;
                    else if (i == 0)
                        returnInt += (int)bytes[i] * 16777216;
                }
            }
            return returnInt;
        }

        /// <summary>
        /// 将int数值转换为占byte数组
        /// </summary>
        /// <param name="value">int</param>
        /// <returns>byte[]</returns>
        private static byte[] intToBytes(int value)
        {
            byte[] src = new byte[2];

            src[0] = (byte)((value >> 8) & 0xFF);
            src[1] = (byte)(value & 0xFF);
            return src;
        }

        /// <summary>
        /// 字符串转数组(1-->49)
        /// </summary>
        /// <param name="str">string</param>
        /// <returns>byte[]</returns>
        private static byte[] strToByte(string str)
        {
            byte[] bytes = new byte[str.Length];
            for (int i = 0; i < str.Length; i++)
            {
                bytes[i] = Convert.ToByte(str[i]);
            }
            return bytes;
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

        public byte[] SetCapTime(int id)
        {
            byte[] cmd = cmdItem.CmdSetCapTimeTemporary;
            if (DateTime.Now.Minute + CapNextTime <= 59)
            {
                cmd[9] = (byte)DateTime.Now.Hour;
                cmd[10] = (byte)(DateTime.Now.Minute + CapNextTime); //当前时刻加5分钟
            }
            else
            {
                //分钟数大于60
                cmd[9] = (byte)(DateTime.Now.Hour + 1);
                cmd[10] = (byte)(DateTime.Now.Minute + CapNextTime - 60 + 1);
            }

            return cmd;
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

    }

}

