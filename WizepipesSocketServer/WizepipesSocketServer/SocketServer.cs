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
        public static Hashtable htIDItem = new Hashtable();
        public static Socket ServerSocket;
        public Queue<byte[]> dataQueue = new Queue<byte[]>();
        public int perPackageLength = 1009;//每包的长度
        public int checkDataQueueTimeInterval = 100;// 检查数据包队列时间休息间隔(ms)
        public bool IsServerOpen = true;

        private ManualResetEvent checkDataQueueResetEvent;//ManualResetEvent:通知一个或多个正在等待的线程已发生事件

        public void OpenServer(string ip, int port)
        {
            try
            {              
                ServerSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                IPEndPoint ipEndPoint = new IPEndPoint(IPAddress.Parse(ip), port);

                //Bind and listen on the given address
                ServerSocket.Bind(ipEndPoint);
                ServerSocket.Listen(100);

                ServerSocket.BeginAccept(new AsyncCallback(OnAccept), null);

                //if (!ThreadPool.QueueUserWorkItem(this.CheckDatagramQueue)) return false;//数据包处理线程
            }
            catch (Exception ex)
            {
                string error = DateTime.Now.ToString() + "出错信息：" + "---" + ex.Message + "\n";
                System.Diagnostics.Debug.WriteLine(error);
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
                    AddressItem addressItem = new AddressItem();
                    addressItem.socket = clientSocket;

                    addressItem.buffer = new byte[perPackageLength];
                    addressItem.strAddress = strAddress;
                    htAddressItem.Add(strAddress, addressItem);
                    //Once the client connects then start receiving the commands from her
                    //开始从连接的socket异步接收数据
                    clientSocket.BeginReceive(addressItem.buffer, 0, addressItem.buffer.Length, SocketFlags.None,
                    new AsyncCallback(OnReceive), clientSocket);
                }
            }
            catch (Exception ex)
            {
                string error = DateTime.Now.ToString() + "出错信息：" + "---" + ex.Message + "\n";
                System.Diagnostics.Debug.WriteLine(error);
            }
        }

        private void OnReceive(IAsyncResult ar)
        {
            string msg = "";
            try
            {
                Socket clientSocket = (Socket)ar.AsyncState;
                string strAddress = clientSocket.RemoteEndPoint.ToString();
                AddressItem addressItem = (AddressItem)htAddressItem[strAddress];//取出当前地址对应的item

                int bytesRead = clientSocket.EndReceive(ar);//接收到的数据长度
                if (bytesRead == 0)
                {
                    CloseSocket();//关闭socket
                }
                else if(addressItem.buffer[0] == 0xA5 && addressItem.buffer[1] == 0xA5 && addressItem.buffer[bytesRead-2] == 0x5A && addressItem.buffer[bytesRead-1] == 0x5A)//判断报文头和尾
                {
                    byte[] recData = new byte[bytesRead];
                    Array.Copy(addressItem.buffer, recData, bytesRead);
                    dataQueue.Enqueue(recData);//Enqueue 将对象添加到 Queue<T> 的结尾处
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
                           
                Thread.Sleep(checkDataQueueTimeInterval);
            }

            checkDataQueueResetEvent.Set();
        }

        private void CloseSocket()
        {

        }



    }
}
