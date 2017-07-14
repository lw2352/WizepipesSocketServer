using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;

namespace WizepipesSocketServer
{
    class DataItem
    {
        public Socket socket;
        public byte[] buffer;
        public string strAddress;
        public Queue<byte[]> dataQueue = new Queue<byte[]>();

        private delegate void AsyncAnalyzeData(byte[] data);

        public void OnReceive(IAsyncResult ar)
        {
            string message = "";
            try
            {
                int bytesRead = socket.EndReceive(ar);//接收到的数据长度
                if (bytesRead == 0)
                {
                    CloseSocket(socket);//关闭socket
                }
                else if (buffer[0] == 0xA5 && buffer[1] == 0xA5 && buffer[bytesRead - 2] == 0x5A && buffer[bytesRead - 1] == 0x5A)//判断报文头和尾
                {
                    byte[] recData = new byte[bytesRead];
                    Array.Copy(buffer, recData, bytesRead);

                    //Console.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + "从地址为"+strAddress+"--收到数据" + byteToHexStr(recData)+"\r\n");

                    dataQueue.Enqueue(recData);//Enqueue 将对象添加到 Queue<T> 的结尾处

                }
                socket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, OnReceive, this);
            }
            catch (Exception ex)
            {
                string error = DateTime.Now.ToString() + "出错信息：" + "---" + ex.Message + "\n";
                System.Diagnostics.Debug.WriteLine(error);
            }
        }

        public void HandleData()
        {
            //Console.WriteLine(DateTime.Now.ToString()+"\r\n");
            if (dataQueue.Count > 0)
            {
                byte[] datagramBytes = dataQueue.Dequeue();// Dequeue 移除并返回位于 Queue<T> 开始处的对象
                AsyncAnalyzeData method = new AsyncAnalyzeData(AnalyzeData);
                method.BeginInvoke(datagramBytes, null, null);
            }
        }

        public void AnalyzeData(byte[] datagramBytes)
        {
            switch (datagramBytes[2])
            {
                case 0x23:
                    Console.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + "收到AD采样数据\r\n");
                    break;
                default:
                    Console.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + "收到其他类型数据\r\n");
                    break;
            }
        }

        public void CloseSocket(Socket socket)
        {
            try
            {
                socket.Shutdown(SocketShutdown.Both);
                socket.Close();
            }
            catch (Exception ex)
            {
                string error = DateTime.Now.ToString() + "出错信息：" + "---" + ex.Message + "\n";
                //Log.Debug(error);
                System.Diagnostics.Debug.WriteLine(error);
            }
        }

        // 字节数组转16进制字符串 
        public static string byteToHexStr(byte[] bytes)
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

    }
}
