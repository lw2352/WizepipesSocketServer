using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;

namespace WizepipesSocketServer
{
    class AddressItem
    {
        public Socket socket;
        public byte[] buffer;
        public string strAddress;
        public Queue<byte[]> dataQueue = new Queue<byte[]>();

        public void OnReceive(IAsyncResult ar)
        {
            string msg = "";
            try
            {
                AddressItem item = ar.AsyncState as AddressItem ;
                int bytesRead = socket.EndReceive(ar);//接收到的数据长度
                if (bytesRead == 0)
                {
                    //CloseSocket();//关闭socket
                }
                else if (buffer[0] == 0xA5 && buffer[1] == 0xA5 && buffer[bytesRead - 2] == 0x5A && buffer[bytesRead - 1] == 0x5A)//判断报文头和尾
                {
                    byte[] recData = new byte[bytesRead];
                    Array.Copy(buffer, recData, bytesRead);
                    dataQueue.Enqueue(recData);//Enqueue 将对象添加到 Queue<T> 的结尾处
                }
                socket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None,
                    new AsyncCallback(OnReceive), this);
            }
            catch (Exception ex)
            {
                string error = DateTime.Now.ToString() + "出错信息：" + "---" + ex.Message + "\n";
                System.Diagnostics.Debug.WriteLine(error);
            }

        }
    }
}
