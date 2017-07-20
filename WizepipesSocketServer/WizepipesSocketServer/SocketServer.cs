using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;


namespace WizepipesSocketServer
{
    class SocketServer
    {
        //TODO:心跳包（掉线重连）等需要for循环哈希表的函数放在server类里面，其余不需要哈希表的放在DataItem类里面;加一个字典<id,socket>,发送命令方便
        

        public static Hashtable htClient = new Hashtable();//strAddress--DataItem
        public static Socket ServerSocket;
        public static Hashtable htSendCmd = new Hashtable();//intID--QueueCmd

        /// <summary>
        /// TODO：做成函数：初始化服务器的输入参数，可以配置
        /// </summary>
        public int perPackageLength = 1009;//每包的长度
        public int checkDataQueueTimeInterval = 100;  // 检查数据包队列时间休息间隔(ms)
        public static int g_datafulllength = 600000; //完整数据包的一个长度
        public static int g_totalPackageCount = 600; //600个包
        public bool IsServerOpen = true;

        private ManualResetEvent checkRecDataQueueResetEvent = new ManualResetEvent(true);//处理接收数据线程；ManualResetEvent:通知一个或多个正在等待的线程已发生事件
        private ManualResetEvent checkSendDataQueueResetEvent = new ManualResetEvent(true);//处理发送数据线程

        public Queue<byte[]> sendDataQueue = new Queue<byte[]>();//数据发送队列

        public bool OpenServer(string ip, int port)
        {
            try
            {
                if (IsServerOpen == true)
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

                    return true;
                }
                else return false;
            }
            catch (Exception ex)
            {
                string error = DateTime.Now.ToString() + "出错信息：" + "---" + ex.Message + "\n";
                System.Diagnostics.Debug.WriteLine(error);
                return false;
            }
        }

        private void OnAccept(IAsyncResult ar)
        {
            try
            {
                Socket clientSocket = ServerSocket.EndAccept(ar);
                //Start listening for more clients
                ServerSocket.BeginAccept(new AsyncCallback(OnAccept), null);

                string strAddress = clientSocket.RemoteEndPoint.ToString();
                if (!htClient.ContainsKey(strAddress))
                {
                    DataItem dataItem = new DataItem();

                    dataItem.Init(clientSocket, perPackageLength, strAddress, 0, g_datafulllength);//初始化dataItem

                    htClient.Add(strAddress, dataItem);
                    //开始从连接的socket异步接收数据
                    dataItem.socket.BeginReceive(dataItem.buffer, 0, dataItem.buffer.Length, SocketFlags.None, OnReceive, dataItem);
                }
            }
            catch (Exception ex)
            {
                string error = DateTime.Now.ToString() + "出错信息：" + "---" + ex.Message + "\n";
                System.Diagnostics.Debug.WriteLine(error);
            }
        }

        public void OnReceive(IAsyncResult ar)
        {
            byte[] ID = new byte[4];//设备的ID号
            DataItem dataItem = ar.AsyncState as DataItem;
            try
            {
                int bytesRead = dataItem.socket.EndReceive(ar);//接收到的数据长度
                if (bytesRead == 0)
                {
                    dataItem.CloseSocket();//关闭socket
                }
                else if (dataItem.buffer[0] == 0xA5 && dataItem.buffer[1] == 0xA5 && dataItem.buffer[bytesRead - 2] == 0x5A && dataItem.buffer[bytesRead - 1] == 0x5A)//判断报文头和尾
                {
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
                            }
                            else
                            {
                                //若存在，把旧地址的status数据属性复制到新地址上
                                DataItem olddataItem = (DataItem) htClient[oldAddress]; //取出旧的dataitem
                                dataItem.status = olddataItem.status;//更新进哈希表
                                htClient.Remove(oldAddress); //删除旧地址的键值对
                            }
                        }

                    }
                    else//心跳包之外的数据在DataItem里面处理
                    {
                        byte[] recData = new byte[bytesRead];
                        Array.Copy(dataItem.buffer, recData, bytesRead);

                        Console.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + "从地址为" +
                                          dataItem.strAddress + "--收到数据" + byteToHexStr(recData) + "\r\n");

                        dataItem.recDataQueue.Enqueue(recData); //Enqueue 将对象添加到 Queue<T> 的结尾处
                    }

                }
                dataItem.socket.BeginReceive(dataItem.buffer, 0, dataItem.buffer.Length, SocketFlags.None, OnReceive, dataItem);
            }
            catch (Exception ex)
            {
                string error = DateTime.Now.ToString() + "出错信息：" + "---" + ex.Message + "\n";
                System.Diagnostics.Debug.WriteLine(error);
            }
        }

        //处理接收队列
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
                Thread.Sleep(checkDataQueueTimeInterval);//当前数据处理线程休眠一段时间
            }
            checkRecDataQueueResetEvent.Set();
        }

        //处理发送队列
        private void CheckSendDataQueue(object state)
        {
            checkSendDataQueueResetEvent.Reset(); //Reset()将事件状态设置为非终止状态，导致线程阻止。
            while (IsServerOpen)
            {
                try
                {
                    foreach (DictionaryEntry de in htSendCmd)
                    {
                        if (htClient.ContainsKey(de.Key))//
                        {
                            Queue<byte[]> queueSendCmd = de.Value as Queue<byte[]>;
                            DataItem dataitem = htClient[de.Key] as DataItem;//取出address对应的dataitem

                        }


                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex);
                }
                Thread.Sleep(checkDataQueueTimeInterval);//当前数据处理线程休眠一段时间
            }
            checkSendDataQueueResetEvent.Set();
        }

        //检查哈希表中是否已存在当前ID
        public string checkIsHaveID(int id)
        {
            foreach (DictionaryEntry de in htClient)
            {
                DataItem dataitem = (DataItem)de.Value;
                if (dataitem.intDeviceID == id)//设备掉线后address(is key)改变而ID号不变
                    return dataitem.strAddress;
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

        //test upload
        public void UploadData()
        {
            foreach (DataItem dataItem in htClient.Values)
            {
                dataItem.status.IsSendDataToServer = true;
                dataItem.status.currentsendbulk = 0;
                dataItem.status.datalength = 0;
            }
        }

        

    }
}
