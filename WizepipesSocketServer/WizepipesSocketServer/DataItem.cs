using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
// [assembly: log4net.Config.XmlConfigurator(Watch = true)]

namespace WizepipesSocketServer
{
    public enum ClientStage
    {
        offLine,//离线
        idle//空闲    
    };

    public enum AdStage
    {
        Idle,//空闲
        AdFinished,//采样完成
        AdUploading,//正在上传
        AdStored//保存成功
    };

    public struct Status
    {
        public bool IsSendDataToServer; //发送数据到服务器
        public bool IsCaptureNow;//是否属于立即采样
        public ClientStage clientStage;
        public AdStage adStage;
        public int currentsendbulk; //当前发送的包数
        public int datalength;  //已保存的AD数据长度
        public byte[] byteAllData; //所有数据，算一个完整的数据
        public DateTime HeartTime; //上一次心跳包发上来的时间
    };

    class DataItem
    {
        public static string Path { get; set; }

        public static log4net.ILog Log = log4net.LogManager.GetLogger(typeof(DataItem));

        public Socket socket;
        public byte[] buffer;
        public string strAddress;
        public int intDeviceID;    //转化为int后的ID       
        public Status status; //设备运行时的属性

        public Queue<byte[]> recDataQueue = new Queue<byte[]>();//数据接收队列；queue是对象的先进先出集合
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
            status.clientStage = ClientStage.idle;
            status.adStage = AdStage.Idle;
            status.currentsendbulk = 0;
            status.byteAllData = new byte[byteAllDataLength];
            status.HeartTime = DateTime.Now;
        }

        public void HandleData()
        {
            if (recDataQueue.Count > 0 && status.clientStage == ClientStage.idle)//命令已发送后，得到返回信息需要一段时间，再去解析数据
            {
                byte[] datagramBytes = recDataQueue.Dequeue();//读取 Queue<T> 开始处的对象并移除
                AsyncAnalyzeData method = new AsyncAnalyzeData(AnalyzeData);
                method.BeginInvoke(datagramBytes, null, null);
            }
        }

        public void SendData()
        {
            if (sendDataQueue.Count > 0 && status.clientStage == ClientStage.idle)//没有待解析的命令，可以去发送命令
            {
                byte[] datagramBytes = sendDataQueue.Dequeue(); //读取 Queue<T> 开始处的对象并移除
                SendCmd(datagramBytes);
            }
        }

        //处理数据和写入数据库
        public void AnalyzeData(byte[] datagramBytes)
        {
            try
            {
                string msg = null;
                switch (datagramBytes[2])
                {
                    case 0x21:
                        if (datagramBytes[7] == 0x00)
                        {
                            msg = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + "硬件" + strAddress + "设备号--" +
                                  intDeviceID + "--读取开启时长和关闭时长成功" + "\n";
                            Console.WriteLine(msg);
                            //ShowMsg(msg);
                        }
                        if (datagramBytes[7] == 0x01)
                        {
                            msg = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + "硬件" + strAddress + "设备号--" +
                                  intDeviceID + "--设定开启时长和关闭时长成功" + "\n";
                            Console.WriteLine(msg);
                            //ShowMsg(msg);
                        }
                        break;

                    case 0x22:
                        if (datagramBytes[9] == 0xAA)
                        {
                            msg = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + "硬件" + strAddress + "设备号--" +
                                  intDeviceID + "--AD采样开始" + "\n";
                            Console.WriteLine(msg);
                            Log.Debug(msg);
                        }
                        if (datagramBytes[9] == 0x55)
                        {
                            status.adStage = AdStage.AdFinished;
                            NetDb.addsensorinfo(intDeviceID, strAddress, strAddress,
                                status.HeartTime.ToString(),
                                Convert.ToInt32(status.clientStage), Convert.ToInt32(status.adStage));

                            msg = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + "硬件" + strAddress + "设备号--" +
                                  intDeviceID + "--AD采样结束" + "\n";
                            Console.WriteLine(msg);
                            Log.Debug(msg);
                            if (status.IsCaptureNow == true)
                            {
                                byte[] CmdAD = { 0xA5, 0xA5, 0x23, 0xFF, 0xFF, 0xFF, 0xFF, 0x00, 0x06, 0x02, 0x57, 0x00, 0x00, 0x03, 0xE8, 0xFF, 0x5A, 0x5A };
                                status.IsSendDataToServer = true;
                                status.currentsendbulk = 0;
                                status.datalength = 0;
                                status.adStage = AdStage.AdUploading;
                                NetDb.addsensorinfo(intDeviceID, strAddress, strAddress,
                                    status.HeartTime.ToString(),
                                    Convert.ToInt32(status.clientStage), Convert.ToInt32(status.adStage));

                                sendDataQueue.Enqueue(CmdAD);
                                msg = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + "硬件" + strAddress + "设备号--" +
                                      intDeviceID + "--AD采样结束,属于立即采样，马上进行上传" + "\n";
                                Log.Debug(msg);
                            }
                        }
                        break;

                    case 0x23:
                        if (datagramBytes.Length >= 1007)
                        {
                            if (status.IsSendDataToServer == true)
                            {
                                status.currentsendbulk++;
                                Console.WriteLine("设备号--" + intDeviceID + "第" + status.currentsendbulk + "包\r\n");

                                if (status.datalength < status.byteAllData.Length)
                                {
                                    // copy data to dataItem
                                    Array.Copy(datagramBytes, 7, status.byteAllData, status.datalength, 1000);
                                    status.datalength += 1000;
                                    Log.Debug("设备号--" + intDeviceID + "存储第" + status.currentsendbulk + "包\r\n");
                                }

                                if (status.currentsendbulk >= 600)
                                {
                                    StoreDataToFile(intDeviceID, status.byteAllData);
                                    //置状态为上传完毕
                                    status.currentsendbulk = 0;
                                    status.IsSendDataToServer = false;
                                    msg = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + "硬件" + strAddress +
                                          "设备号--" + intDeviceID + "--数据上传完毕" + "\n";
                                    Console.WriteLine(msg);
                                    Log.Debug(msg);
                                    status.adStage = AdStage.AdStored;//上传完成
                                    NetDb.addsensorinfo(intDeviceID, strAddress, strAddress,
                                        status.HeartTime.ToString(),
                                        Convert.ToInt32(status.clientStage), Convert.ToInt32(status.adStage));
                                }

                            }
                        }
                        break;

                    case 0x25:
                        if (datagramBytes[9] == 0x55)
                        {
                            msg = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + "硬件" + strAddress + "设备号--" +
                                  intDeviceID + "--设定GPS采样时间成功" + "\n";
                            Console.WriteLine(msg);
                            Log.Debug(msg);
                            NetDb.UpdateSensorCfg(intDeviceID, "IsSetCapTime", 0);
                            //NetDb.UpdateSensorCfg(intDeviceID, "IsCaptureNow", 0); 
                        }

                        break;

                    case 0x26:
                        if (datagramBytes[7] == 0x01)
                        {
                            msg = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + "硬件" + strAddress + "设备号--" +
                                  intDeviceID + "--设定开启时长和关闭时长成功" + "\n";
                            Console.WriteLine(msg);
                            Log.Debug(msg);
                            NetDb.UpdateSensorCfg(intDeviceID, "IsSetOpenAndCloseTime", 0);
                        }
                        break;

                    case 0x27:
                        int[] gpsData = new int[23];
                        double Latitude = 0;
                        double Longitude = 0;
                        for (int i = 0; i < 23; i++)
                        {
                            gpsData[i] = datagramBytes[9 + i];
                        }
                        GPSDistance.getGPSData(gpsData, out Latitude, out Longitude);
                        msg = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + "硬件" + strAddress + "设备号--" + intDeviceID + "--经度为：" + Longitude + "纬度为：" + Latitude + "\n";
                        Console.WriteLine(msg);
                        Log.Debug(msg);

                        //把经纬度写入数据库;在addsensorcfg后面加上IsGetGPSinfo，hex加到3
                        if (Longitude > 0 && Latitude > 0)
                        {//保留6位小数
                            Longitude = (int) (Longitude * 1000000);
                            Longitude = Longitude / 1000000;

                            Latitude = (int) (Latitude * 1000000);
                            Latitude = Latitude / 1000000;

                            NetDb.UpdateSensorGPSinfo(intDeviceID, Longitude, Latitude);
                        }
                        else
                        {
                            NetDb.UpdateSensorGPSinfo(intDeviceID, -1, -1);
                        }
                        //经纬度信息读取成功,把标志位复位
                        NetDb.UpdateSensorCfg(intDeviceID, "IsGetGpsInfo", 0);
                        Log.Debug("经纬度信息读取成功,把标志位复位");
                        break;

                    case 0x28:
                        if (datagramBytes[9] == 0x55)
                        {
                            msg = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + "硬件" + strAddress + "设备号--" +
                                  intDeviceID + "--设定立即采样时间成功" + "\n";
                            Console.WriteLine(msg);
                            Log.Debug(msg);
                            NetDb.UpdateSensorCfg(intDeviceID, "IsCaptureNow", 0);
                        }

                        break;

                    case 0x29:
                        msg = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + "硬件" + strAddress + "设备号--" +
                              intDeviceID + "--设定服务器IP成功" + "\n";
                        Console.WriteLine(msg);
                        Log.Debug(msg);
                        NetDb.UpdateSensorCfg(intDeviceID, "IsSetServerIP", 0);
                        break;

                    case 0x30:
                        msg = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + "硬件" + strAddress + "设备号--" +
                              intDeviceID + "--设定服务器端口号成功" + "\n";
                        Console.WriteLine(msg);
                        Log.Debug(msg);
                        NetDb.UpdateSensorCfg(intDeviceID, "IsSetServerPort", 0);
                        break;

                    case 0x31:
                        msg = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + "硬件" + strAddress + "设备号--" +
                              intDeviceID + "--设定AP名称成功" + "\n";
                        Console.WriteLine(msg);
                        Log.Debug(msg);
                        NetDb.UpdateSensorCfg(intDeviceID, "IsSetApName", 0);
                        break;

                    case 0x32:
                        msg = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + "硬件" + strAddress + "设备号--" +
                              intDeviceID + "--设定AP密码成功" + "\n";
                        Console.WriteLine(msg);
                        Log.Debug(msg);
                        NetDb.UpdateSensorCfg(intDeviceID, "IsSetApPassword", 0);
                        break;

                    case 0x33:
                        msg = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + "硬件" + strAddress + "设备号--" +
                              intDeviceID + "--重连设定完成" + "\n";
                        Console.WriteLine(msg);
                        Log.Debug(msg);
                        NetDb.UpdateSensorCfg(intDeviceID, "IsReconnect", 0);
                        NetDb.UpdateSensorInfo(intDeviceID, "Status", 0);//接收到8266的重连确认信息后，置设备状态为不在线0
                        break;

                    case 0xFF:
                        status.clientStage = ClientStage.idle;
                        status.HeartTime = DateTime.Now;
                        NetDb.UpdateSensorInfo(intDeviceID, "Status", Convert.ToInt32(status.clientStage));
                        NetDb.UpdateSensorInfoWithTime(intDeviceID, "loginTime", status.HeartTime.ToString());
                        Console.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + "设备号--" +
                            intDeviceID + "收到心跳包\r\n");
                        Log.Debug("设备号--" + intDeviceID + "收到心跳包\r\n");
                        break;
                    default:
                        Console.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + "收到其他类型数据\r\n");
                        Log.Debug("收到其他类型数据");
                        break;
                }

                if (status.IsSendDataToServer == true)
                {
                    SendCmd(SetADcmd(status.currentsendbulk));//继续发送AD命令
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Log.Debug(e);
            }
        }


        private void SendCmd(byte[] cmd)
        {
            try
            {
                socket.BeginSend(cmd, 0, cmd.Length, SocketFlags.None, new AsyncCallback(OnSend), this);
                Console.WriteLine(DateTime.Now + "向设备号是--" + intDeviceID + "--发送的命令是" + byteToHexStr(cmd) + ";\n");
            }
            catch (Exception ex)
            {
                //socket无效，发送命令失败
                Console.WriteLine(ex);
                CloseSocket();
                status.clientStage = ClientStage.offLine;
                Log.Debug(ex);
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
            }
            catch (Exception ex)
            {
                Log.Debug(ex);
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
                Log.Debug(ex);
                string error = DateTime.Now.ToString() + "出错信息：" + "--服务端主动断开连接--" + ex.Message + "\n";
                System.Diagnostics.Debug.WriteLine(error);
            }
        }


        /// <summary>
        /// 构造AD采样命令
        /// </summary>
        /// <param name="bulkCount">包数</param>
        /// <returns>string</returns>
        private byte[] SetADcmd(int bulkCount)
        {
            byte[] Cmd = { 0xA5, 0xA5, 0x23, 0xFF, 0xFF, 0xFF, 0xFF, 0x00, 0x06, 0x02, 0x57, 0x00, 0x00, 0x03, 0xE8, 0xFF, 0x5A, 0x5A };
            byte[] bytesbulkCount = new byte[2];
            bytesbulkCount = intToBytes(bulkCount);

            Cmd[11] = bytesbulkCount[0];
            Cmd[12] = bytesbulkCount[1];

            return (Cmd);
        }

        /// <summary>
        /// 将int数值转换为占byte数组
        /// </summary>
        /// <param name="value">int</param>
        /// <returns>byte[]</returns>
        private byte[] intToBytes(int value)
        {
            byte[] src = new byte[2];

            src[0] = (byte)((value >> 8) & 0xFF);
            src[1] = (byte)(value & 0xFF);
            return src;
        }

        //保存文件，16进制，封装开头是0xAA，结尾是0x55
        private void StoreDataToFile(int intDeviceID, byte[] bytes)
        {
            string filename = DateTime.Now.ToString("yyyy-MM-dd") + "--" + DateTime.Now.Hour.ToString() + "-" + DateTime.Now.Minute.ToString() + "-" + DateTime.Now.Second.ToString() + "--" + intDeviceID.ToString();//以日期时间命名，避免文件名重复
            byte[] fileStartAndEnd = new byte[2] { 0xAA, 0x55 };//保存文件的头是AA，尾是55
            string url = @Path;

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
                NetDb.addsensorad(intDeviceID, DateTime.Now.ToString(), path);
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
                NetDb.addsensorad(intDeviceID, DateTime.Now.ToString(), path);
            }
        }

        // 字节数组转16进制字符串 
        private string byteToHexStr(byte[] bytes)
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
