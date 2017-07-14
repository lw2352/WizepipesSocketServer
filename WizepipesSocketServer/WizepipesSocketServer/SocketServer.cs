using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;

//TODO:在item类里面存socket和地址等值，也存发送函数等，那样就可以直接发送，不用匹配id和socket


namespace WizepipesSocketServer
{
    class SocketServer
    {    
        public static Hashtable htAddressItem = new Hashtable();
        public static Socket ServerSocket;
        
        public int perPackageLength = 1009;//每包的长度
        public int checkDataQueueTimeInterval = 100;// 检查数据包队列时间休息间隔(ms)
        public bool IsServerOpen = true;
        private ManualResetEvent checkDataQueueResetEvent = new ManualResetEvent(true);//ManualResetEvent:通知一个或多个正在等待的线程已发生事件

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

                    //数据包处理线程
                    if (!ThreadPool.QueueUserWorkItem(this.CheckDatagramQueue))
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
                if (!htAddressItem.ContainsKey(strAddress))
                {
                    DataItem dataItem = new DataItem();
                    dataItem.socket = clientSocket;

                    dataItem.buffer = new byte[perPackageLength];
                    dataItem.strAddress = strAddress;
                    htAddressItem.Add(strAddress, dataItem);
                    //Once the client connects then start receiving the commands from her
                    //开始从连接的socket异步接收数据
                    dataItem.socket.BeginReceive(dataItem.buffer, 0, dataItem.buffer.Length, SocketFlags.None, dataItem.OnReceive, dataItem);
                }
            }
            catch (Exception ex)
            {
                string error = DateTime.Now.ToString() + "出错信息：" + "---" + ex.Message + "\n";
                System.Diagnostics.Debug.WriteLine(error);
            }
        }


        private void CheckDatagramQueue(object state)
        {
            checkDataQueueResetEvent.Reset(); //Reset()将事件状态设置为非终止状态，导致线程阻止。
            while (IsServerOpen)
            {
                try
                {
                    foreach (DataItem dataItem in htAddressItem.Values)
                    {
                        dataItem.HandleData();
                    }
                }
                catch (Exception ex)
                { System.Diagnostics.Debug.WriteLine(ex); }
                Thread.Sleep(checkDataQueueTimeInterval);//当前数据处理线程休眠一段时间
            }
            
            checkDataQueueResetEvent.Set();
        }

    }
}
