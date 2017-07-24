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
        public static log4net.ILog DebugLog = log4net.LogManager.GetLogger(typeof(SocketServer));
        public static Hashtable htClient = new Hashtable();//strAddress--DataItem
        public static Socket ServerSocket;
        public static Hashtable htSendCmd = new Hashtable();//intID--QueueCmd
        public static CmdItem cmdItem = new CmdItem();//实例化

        public int perPackageLength = 1009;//每包的长度
        public int checkRecDataQueueTimeInterval = 10;  // 检查接收数据包队列时间休息间隔(ms)
        public int checkSendDataQueueTimeInterval = 5000;  // 检查发送命令队列时间休息间隔(ms)
        public int checkDataBaseQueueTimeInterval = 15000;  // 检查数据库命令队列时间休息间隔(ms)
        public static int g_datafulllength = 600000; //完整数据包的一个长度
        public static int g_totalPackageCount = 600; //600个包
        public bool IsServerOpen = true;
        public int maxTimeOut = 240;//超时未响应时间--120s
        public int maxBadClient = 1;//最大的故障设备数

        private ManualResetEvent checkRecDataQueueResetEvent = new ManualResetEvent(true);//处理接收数据线程；ManualResetEvent:通知一个或多个正在等待的线程已发生事件
        private ManualResetEvent checkSendDataQueueResetEvent = new ManualResetEvent(true);//处理发送数据线程，把数据哈希表中的数据复制到各个dataItem中的发送队列
        private ManualResetEvent CheckDataBaseQueueResetEvent = new ManualResetEvent(true);

        //初始化服务器，给服务参数赋值
        public void InitServer()
        {
            string ConfigIp = System.Configuration.ConfigurationManager.AppSettings["ServerIP"];
            Console.WriteLine("从config文件读取的IP为：" + ConfigIp);
            string DB = System.Configuration.ConfigurationManager.AppSettings["ServerDB"];
            MySQLDB.strDbConn = DB;
        }

        public bool OpenServer(string ip, int port)
        {
            try
            {
                
                    ServerSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    IPEndPoint ipEndPoint = new IPEndPoint(IPAddress.Parse(ip), port);

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

                    return true;
            }
            catch (Exception ex)
            {
                DebugLog.Debug(ex);
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
                //TODO:GC
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                DebugLog.Debug(ex);
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
                    ServerSocket.BeginAccept(new AsyncCallback(OnAccept), null);

                    string strAddress = clientSocket.RemoteEndPoint.ToString();
                    if (!htClient.ContainsKey(strAddress))
                    {
                        DataItem dataItem = new DataItem();

                        dataItem.Init(clientSocket, perPackageLength, strAddress, 0, g_datafulllength); //初始化dataItem

                        htClient.Add(strAddress, dataItem);

                        Console.WriteLine(DateTime.Now.ToString() + "收到客户端" + strAddress + "的连接请求" + "\n");

                        //开始从连接的socket异步接收数据
                        dataItem.socket.BeginReceive(dataItem.buffer, 0, dataItem.buffer.Length, SocketFlags.None,
                            OnReceive, clientSocket);
                    }
                }
            }
            catch (Exception ex)
            {
                DebugLog.Debug(ex);
                string error = DateTime.Now.ToString() + "出错信息：" + "---" + ex.Message + "\n";
                System.Diagnostics.Debug.WriteLine(error);
            }
        }

        public void OnReceive(IAsyncResult ar)
        {
            byte[] ID = new byte[4];//设备的ID号
            int bytesRead = 0;
            try
            {
                Socket clientSocket = ar.AsyncState as Socket;
                string strAddress = clientSocket.RemoteEndPoint.ToString();
                DataItem dataItem = (DataItem)htClient[strAddress];//取出address对应的dataitem
                if (clientSocket.Connected) //检测socket上一次的状态
                {
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
                        recData = null;

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
                                        Convert.ToInt32(dataItem.status.clientStage));
                                }
                                else
                                {
                                    //若存在，把旧地址的status数据属性复制到新地址上
                                    DataItem olddataItem = (DataItem) htClient[oldAddress]; //取出旧的dataitem
                                    //更新进哈希表
                                    dataItem.intDeviceID = intdeviceID;
                                    dataItem.status = olddataItem.status;
                                    dataItem.status.clientStage = ClientStage.idle;
                                    dataItem.recDataQueue = olddataItem.recDataQueue;
                                    dataItem.sendDataQueue = olddataItem.sendDataQueue;

                                    //把信息存入数据库
                                    NetDb.addsensorinfo(intdeviceID, dataItem.strAddress, dataItem.strAddress,
                                        dataItem.status.HeartTime.ToString(),
                                        Convert.ToInt32(dataItem.status.clientStage));

                                    htClient.Remove(oldAddress); //删除旧地址的键值对
                                }
                            }

                        } // if (dataItem.buffer[2] == 0xFF)
                    }
                }
                dataItem.socket.BeginReceive(dataItem.buffer, 0, dataItem.buffer.Length, SocketFlags.None, OnReceive, dataItem);
            }
            catch (Exception ex)
            {
                //客户端强制断开连接
                
                DebugLog.Debug(ex);            
                string error = DateTime.Now.ToString() + "出错信息：" + "---" + ex.Message + "\n";
                System.Diagnostics.Debug.WriteLine(error);
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
                }
                Thread.Sleep(checkRecDataQueueTimeInterval);//当前数据处理线程休眠一段时间
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
                    int AdFinishedClientNum = 0;//发送采样完成信息的设备数
                    //int AdUploadedClientNum = 0;

                    foreach (DataItem dataItem in htClient.Values)
                    {
                        dataItem.SendData();
                        //dataItem.CheckTimeout(maxTimeOut);
                        if (dataItem.status.adStage == AdStage.AdFinished)
                        {
                            AdFinishedClientNum++;
                        }
                        //else if (dataItem.status.adStage == AdStage.AdUploaded)
                        //{
                            //AdUploadedClientNum++;
                        //}
                        if (htSendCmd.ContainsKey(dataItem.intDeviceID))//发送命令哈希表中是否包含当前dataItem的id
                        {
                            Queue<byte[]> sendCmdQueue = htSendCmd[dataItem.intDeviceID] as Queue<byte[]>;
                            while (sendCmdQueue != null && sendCmdQueue.Count > 0)
                            {
                                dataItem.sendDataQueue.Enqueue(sendCmdQueue.Dequeue());//复制数据
                            }
                            //htSendCmd.Remove(dataItem.intDeviceID);//不需要移除，等count减到0后，上面的if判断自然进不去
                        }
                    }

                    if (AdFinishedClientNum >= (htClient.Count - maxBadClient) && (htClient.Count > maxBadClient))
                    {
                        //开始上传
                        Console.WriteLine("开始上传");
                        UploadData();
                    }

                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex);
                }
                Thread.Sleep(checkSendDataQueueTimeInterval);//当前数据处理线程休眠一段时间
            }
            checkSendDataQueueResetEvent.Set();
        }

        //读取数据库命令线程
        public void CheckDataBaseQueue(object state)
        {
            int[] cfg = new int[5];//存储从数据库读取的设备配置参数
            // TODO:收到设备数据后写数据库，表示有回复（发送成功），不再重复发送
            //test
            while (IsServerOpen)
            {
                try
                {
                    foreach (DataItem dataItem in htClient.Values)
                    {
                        cfg = NetDb.readsensorcfg(dataItem.intDeviceID); //从数据库读取设备的配置参数
                        if (cfg != null)
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
                            if (!htSendCmd.ContainsKey(dataItem.intDeviceID))
                            {
                                htSendCmd.Add(dataItem.intDeviceID, DbCmdQueue);
                                Console.WriteLine("htSendCmd添加命令队列成功");
                            }
                            
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
                Thread.Sleep(checkDataBaseQueueTimeInterval);
            }
            CheckDataBaseQueueResetEvent.Set();
        }

        //检查哈希表中是否已存在当前ID
        public string checkIsHaveID(int id)
        {
            foreach (DataItem dataItem in htClient.Values)
            {
                if (dataItem.intDeviceID == id)//设备掉线后address(is key)改变而ID号不变
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
         
        public void UploadData()
        {
            byte[] CmdAD = cmdItem.CmdADPacket;
            foreach (DataItem dataItem in htClient.Values)
            {
                dataItem.status.IsSendDataToServer = true;
                dataItem.status.currentsendbulk = 0;
                dataItem.status.datalength = 0;
                dataItem.status.adStage = AdStage.AdUploading;
                dataItem.sendDataQueue.Enqueue(CmdAD);
            }
        }

        //test for set capTime
        public void AddCmdToQueue(int id, byte[] cmd)
        {
            if (id == 0xFF)
            {
                foreach (DataItem dataItem in htClient.Values)
                {
                    dataItem.sendDataQueue.Enqueue(cmd);
                }
            }
            //Queue<byte[]> sendCmdQueue = htSendCmd[id] as Queue<byte[]>;
            //sendCmdQueue.Enqueue(cmd);
        }

    }
}
