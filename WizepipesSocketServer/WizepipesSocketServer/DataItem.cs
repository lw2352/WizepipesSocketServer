using System;
using System.Collections.Generic;
using System.IO;
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
        public Queue<byte[]> sendDataQueue = new Queue<byte[]>();//数据发送队列
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

        public void SendData()
        {
            if (recDataQueue.Count > 0)
            {
                byte[] datagramBytes = recDataQueue.Peek(); // Dequeue 返回位于 Queue<T> 开始处的对象但不移除
                SendCmd(datagramBytes);
                //TODO:发送一条命令后，记录命令号，得到相应的返回信息后再移除队列中的信息；还要加互斥信号量。
            }
        }

        //处理数据和写入数据库
        public void AnalyzeData(byte[] datagramBytes)
        {
            string msg = "";
            switch (datagramBytes[2])
            {
                case 0x21:
                    if (datagramBytes[7] == 0x00)
                    {
                        msg = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + "硬件" + strAddress + "设备号--" + intDeviceID + "--读取开启时长和关闭时长成功" + "\n";
                        Console.WriteLine(msg);
                        //ShowMsg(msg);
                    }
                    if (buffer[7] == 0x01)
                    {
                        msg = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + "硬件" + strAddress + "设备号--" + intDeviceID + "--设定开启时长和关闭时长成功" + "\n";
                        Console.WriteLine(msg);
                        //ShowMsg(msg);
                    }
                    break;

                case 0x22:
                    if (buffer[9] == 0xAA)
                    {
                        msg = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + "硬件" + strAddress + "设备号--" + intDeviceID + "--AD采样开始" + "\n";
                    }
                    else if (buffer[9] == 0x55)
                    {
                        //dataitem.CmdStage = 1;
                        msg = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + "硬件" + strAddress + "设备号--" + intDeviceID + "--AD采样结束" + "\n";
                    }
                    Console.WriteLine(msg);
                    //ShowMsg(msg);
                    break;

                case 0x25:
                    if (buffer[9] == 0x55)
                    {
                        msg = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + "硬件" + strAddress + "设备号--" + intDeviceID + "--设定GPS采样时间成功" + "\n";
                        Console.WriteLine(msg);
                        //ShowMsg(msg);
                    }

                    break;

                case 0x26:
                    if (buffer[7] == 0x01)
                    {
                        msg = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + "硬件" + strAddress + "设备号--" + intDeviceID + "--设定开启时长和关闭时长成功" + "\n";
                        Console.WriteLine(msg);
                       // ShowMsg(msg);
                    }
                    break;

                case 0x27:
                    int[] gpsData = new int[23];
                    for (int i = 0; i < 23; i++)
                    {
                        //gpsData[i] = dataitem.SingleBuffer[9 + i];
                    }
                    //gpsDistance.getGPSData(gpsData, out dataitem.Latitude, out dataitem.Longitude);
                    //msg = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + "硬件" + strAddress + "设备号--" + intDeviceID + "--经度为：" + dataitem.Longitude + "纬度为：" + dataitem.Latitude + "\n";
                    Console.WriteLine(msg);
                    //ShowMsg(msg);
                    break;

                case 0x29:
                    msg = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + "硬件" + strAddress + "设备号--" + intDeviceID + "--设定服务器IP成功" + "\n";
                    Console.WriteLine(msg);
                    //ShowMsg(msg);
                    break;

                case 0x30:
                    msg = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + "硬件" + strAddress + "设备号--" + intDeviceID + "--设定服务器端口号成功" + "\n";
                    Console.WriteLine(msg);
                    //ShowMsg(msg);
                    break;

                case 0x31:
                    msg = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + "硬件" + strAddress + "设备号--" + intDeviceID + "--设定AP名称成功" + "\n";
                    Console.WriteLine(msg);
                    //ShowMsg(msg);
                    break;

                case 0x32:
                    msg = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + "硬件" + strAddress + "设备号--" + intDeviceID + "--设定AP密码成功" + "\n";
                    Console.WriteLine(msg);
                    //ShowMsg(msg);
                    break;

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
                                StoreDataToFile(intDeviceID, status.byteAllData);
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


        private void SendCmd(byte[] cmd)
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

        //保存文件，16进制，封装开头是0xAA，结尾是0x55
        private void StoreDataToFile(int intDeviceID, byte[] bytes)
        {
            string filename = DateTime.Now.ToString("yyyy-MM-dd") + "--" + DateTime.Now.Hour.ToString() + "-" + DateTime.Now.Minute.ToString() + "-" + DateTime.Now.Second.ToString() + "--" + intDeviceID.ToString();//以日期时间命名，避免文件名重复
            byte[] fileStartAndEnd = new byte[2] { 0xAA, 0x55 };//保存文件的头是AA，尾是55
            string url = @"D:\\Data";

            if (!Directory.Exists(url))//如果不存在就创建file文件夹　　             　　                
            {
                Directory.CreateDirectory(url);//创建该文件夹　

                string path = url + filename + ".dat";
                FileStream F = new FileStream(path, FileMode.OpenOrCreate, FileAccess.ReadWrite);
                F.Write(fileStartAndEnd, 0, 1);
                F.Write(bytes, 0, bytes.Length);
                F.Write(fileStartAndEnd, 1, 1);
                F.Flush();
                F.Close();
            }
            else
            {
                string path = url + filename + ".dat";
                FileStream F = new FileStream(path, FileMode.OpenOrCreate, FileAccess.ReadWrite);
                F.Write(fileStartAndEnd, 0, 1);
                F.Write(bytes, 0, bytes.Length);
                F.Write(fileStartAndEnd, 1, 1);
                F.Flush();
                F.Close();
            }
        }

    }
}
