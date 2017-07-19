using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;

namespace WizepipesSocketServer
{
    public enum Stage
    {
        stage0,    // stage0
        stage1  // stage1
    };

    public struct Status
    {
        public bool IsSendDataToServer; //发送数据到服务器
        public Stage stage;
        public int currentsendbulk; //当前发送的包数
        public int datalength;  //已保存的AD数据长度
        public byte[] byteAllData; //所有数据，算一个完整的数据
    };

    class DataItem
    {
        public Socket socket;
        public byte[] buffer;
        public string strAddress;
        public int intDeviceID;    //转化为int后的ID       
                 
        public Status status; //设备运行时的属性

        public Queue<byte[]> recDataQueue = new Queue<byte[]>();//数据接收队列
        private delegate void AsyncAnalyzeData(byte[] data);
        /// <summary>
        /// 初始化DataItem
        /// </summary>
        /// <param name="clientsocket"></param>
        /// <param name="bufferLength"></param>
        /// <param name="address"></param>
        /// <param name="ID"></param>
        /// <param name="dataLength"></param>
        public void Init(Socket clientsocket, int bufferLength, string address, int ID, int byteAllDataLength)
        {
            socket = clientsocket;
            buffer = new byte[bufferLength];
            strAddress = address;
            intDeviceID = ID;

            status.IsSendDataToServer = false;
            status.stage = 0;
            status.currentsendbulk = 0;
            status.byteAllData = new byte[byteAllDataLength];
        }

        public void HandleData()
        {
            if (recDataQueue.Count > 0)
            {
                byte[] datagramBytes = recDataQueue.Dequeue();// Dequeue 移除并返回位于 Queue<T> 开始处的对象
                AsyncAnalyzeData method = new AsyncAnalyzeData(AnalyzeData);
                method.BeginInvoke(datagramBytes, null, null);
            }
        }

        public void AnalyzeData(byte[] datagramBytes)
        {
            switch (datagramBytes[2])
            {
                case 0x23:
                    if (datagramBytes.Length >= 1007)
                    {
                        if (status.IsSendDataToServer == true)
                        {
                            status.currentsendbulk++;
                            // copy data to dataItem
                            Array.Copy(datagramBytes, 7, status.byteAllData, status.datalength, 1000);
                            status.datalength += 1000;

                            if (status.currentsendbulk == 600)
                            {
                                //StoreDataToFile(dataItem.intDeviceID, dataItem.status.byteAllData);
                                //置状态为上传完毕
                                status.currentsendbulk = 0;
                                status.IsSendDataToServer = false;
                            }
                        }
                    }
                    Console.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + "收到AD采样数据\r\n");
                    break;


                default:
                    Console.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + "收到其他类型数据\r\n");
                    break;
            }
        }


        public void SendData(byte[] cmd)
        {
            try
            {
                socket.BeginSend(cmd, 0, cmd.Length, SocketFlags.None, new AsyncCallback(OnSend), this);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                CloseSocket();
            }
        }

        /// <summary>
        /// 异步发送数据
        /// </summary>
        /// <param name="ar">IAsyncResult</param>
        private void OnSend(IAsyncResult ar)
        {
            try
            {
                socket.EndSend(ar);
                ar.AsyncWaitHandle.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        public void CloseSocket()
        {
            try
            {
                socket.Shutdown(SocketShutdown.Both);
                socket.Close();
            }
            catch (Exception ex)
            {
                string error = DateTime.Now.ToString() + "出错信息：" + "---" + ex.Message + "\n";
                System.Diagnostics.Debug.WriteLine(error);
            }
        }

    }
}
