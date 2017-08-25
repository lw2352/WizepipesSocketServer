using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace WizepipesSocketServer
{
    public partial class MainForm : Form
    {
        SocketServer server = new SocketServer();


        public MainForm()
        {
            InitializeComponent();
            ReadCfg();
            
        }

        public void ReadCfg()
        {
            string msg = null;

            string ConfigIp = System.Configuration.ConfigurationManager.AppSettings["ServerIP"];
            string ConfigPort = System.Configuration.ConfigurationManager.AppSettings["ServerPort"];
            string ConfigbufferLength = System.Configuration.ConfigurationManager.AppSettings["bufferLength"];
            string ConfigAdlength = System.Configuration.ConfigurationManager.AppSettings["Adlength"];
            string ConfigcheckRecDataQueueTimeInterval =
                System.Configuration.ConfigurationManager.AppSettings["checkRecDataQueueTimeInterval"];
            string ConfigcheckSendDataQueueTimeInterval =
                System.Configuration.ConfigurationManager.AppSettings["checkSendDataQueueTimeInterval"];
            string ConfigcheckDataBaseQueueTimeInterval =
                System.Configuration.ConfigurationManager.AppSettings["checkDataBaseQueueTimeInterval"];
            string Configg_totalPackageCount =
                System.Configuration.ConfigurationManager.AppSettings["g_totalPackageCount"];
            string ConfigmaxTimeOut = System.Configuration.ConfigurationManager.AppSettings["maxTimeOut"];
            string ConfigmaxBadClient = System.Configuration.ConfigurationManager.AppSettings["maxBadClient"];
            string ConfigIsAutoTest = System.Configuration.ConfigurationManager.AppSettings["IsAutoTest"];
            string ConfigCapNextTime = System.Configuration.ConfigurationManager.AppSettings["CapNextTime"];
            string DB = System.Configuration.ConfigurationManager.AppSettings["ServerDB"];
            string ImagePath = System.Configuration.ConfigurationManager.AppSettings["ImagePath"];

            msg = "从appConfig读取到的配置信息是：" + "ServerIP:" + ConfigIp + "\r\nServerPort:" + ConfigPort +
                  "\r\nbufferLength:" + ConfigbufferLength + "\r\nAdlength:" + ConfigAdlength +
                  "\r\n接收数据线程休息时间：" + ConfigcheckRecDataQueueTimeInterval + "\r\n发送数据线程休息时间：" +
                  ConfigcheckSendDataQueueTimeInterval +
                  "\r\n读取数据库线程休息时间：" + ConfigcheckDataBaseQueueTimeInterval
                  + "\r\n采样数据的总包数：" + Configg_totalPackageCount + "\r\n超时断开最大时长：" + ConfigmaxTimeOut + "\r\n允许故障设备数：" +
                  ConfigmaxBadClient + "\r\n是否自动测试：" + ConfigIsAutoTest + "\r\n数据库连接字符串：" + DB + "\r\n自动采样的间隔时长" +
                  ConfigCapNextTime;

            richTextBox1.AppendText(msg);
            //共12+1个
            server.CfgServerIP = ConfigIp;
            server.CfgServerPort = Convert.ToInt32(ConfigPort);
            server.CfgbufferLength = Convert.ToInt32(ConfigbufferLength);
            server.CfgAdlength = Convert.ToInt32(ConfigAdlength);
            server.CfgcheckRecDataQueueTimeInterval = Convert.ToInt32(ConfigcheckRecDataQueueTimeInterval);
            server.CfgcheckSendDataQueueTimeInterval = Convert.ToInt32(ConfigcheckSendDataQueueTimeInterval);
            server.CfgcheckDataBaseQueueTimeInterval = Convert.ToInt32(ConfigcheckDataBaseQueueTimeInterval);
            server.Cfgg_totalPackageCount = Convert.ToInt32(Configg_totalPackageCount);
            server.CfgmaxTimeOut = Convert.ToInt32(ConfigmaxTimeOut);
            server.CfgmaxBadClient = Convert.ToInt32(ConfigmaxBadClient);
            server.CfgIsAutoTest = Convert.ToInt32(ConfigIsAutoTest);
            server.CfgCapNextTime = Convert.ToInt32(ConfigCapNextTime);
            MySQLDB.strDbConn = DB;
            ZedToPng.Path = ImagePath;

        }

        private void button1_Click(object sender, EventArgs e)
        {
            server.OpenServer();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            server.UploadData();
        }

        private void buttonCapNow_Click(object sender, EventArgs e)
        {
            if (checkBoxAutoTest.Checked == true)
            {
                server.CfgIsAutoTest = 1;
            }
            else
            {
                server.CfgIsAutoTest = 0;
            }
            server.SetCapTime(0xFF);
        }

        private void buttonCloseServer_Click(object sender, EventArgs e)
        {
            server.CloseServer();
        }

        private void buttonViewClientInfo_Click(object sender, EventArgs e)
        {
            string msg = server.ViewClientInfo();
            if (msg != null)
                richTextBox1.AppendText(msg);
        }

        private void buttonClear_Click(object sender, EventArgs e)
        {
            richTextBox1.Clear();
        }

        private void buttonAnalyze_Click(object sender, EventArgs e)
        {
            //server.CalculateOffset(7, 5, 1000, 5000);
            //List<int> testList = NetDb.GetpipeInfo(3,4);
            //string name = NetDb.GetSensorName(3);
            /*byte[] CmdReadCurrentOpenAndCloseTime = new byte[] { 0xA5, 0xA5, 0x21, 0xFF, 0xFF, 0xFF, 0xFF, 0x00, 0x04, 0x00, 0x00, 0x00, 0x00, 0xFF, 0x5A, 0x5A };
            List<byte[]> AnalyzeList = new List<byte[]>();
            AnalyzeList.Add(CmdReadCurrentOpenAndCloseTime);
            byte test = AnalyzeList[0][0];

            Hashtable htTest = new Hashtable();
            htTest.Add(CmdReadCurrentOpenAndCloseTime, AnalyzeList);
            if (htTest.Contains(CmdReadCurrentOpenAndCloseTime))
            {
                Console.WriteLine("ok");
            }*/

            /*int[,] a = new int[3, 4] {
                {0, 1, 2, 3} ,   /*  初始化索引号为 0 的行 */
            //{4, 5, 6, 7} ,   /*  初始化索引号为 1 的行 */
            //{8, 9, 10, 11}   /*  初始化索引号为 2 的行 */
            // };

            //int i=a.GetLength(0);//行数
            //int j = a.GetLength(1);//列数
            List<int[]> AreaDeviceList = NetDb.readAllDeviceIdByAreaID();


            NetDb.UpdateLeakPointScale(5,0.456123);

            string distance = "3.9";
            if (Convert.ToDouble(distance) > 0 && Convert.ToDouble(distance) < 5)
            {
                distance = null;
            }

            NetDb.UpdateLeakTimes(3);

            double Longitude = 114.12456789;
            Longitude = (int)(Longitude * 1000000);
            Longitude = Longitude / 1000000;

            byte[] CmdSetCapTime = new byte[] { 0xA5, 0xA5, 0x25, 0xFF, 0xFF, 0xFF, 0xFF, 0x01, 0x30,
                0x00, 0x00, 0x00, 0x00,0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00,0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00,0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00,0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00,0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00,0x00, 0x00, 0x00, 0x00,
                0xFF, 0x5A, 0x5A };

            List<byte[]> cfgList = new List<byte[]>();
            //cfgList = NetDb.readsensorcfg(3);
            cfgList.Add(null);
            cfgList.Add(null);
            cfgList.Add(null);

            cfgList[0] = CmdSetCapTime;
            cfgList[2] = CmdSetCapTime;


            int[,] a = new int[3, 4] {
                {0, 1, 2, 3} ,   /*  初始化索引号为 0 的行 */
                {4, 5, 6, 7} ,   /*  初始化索引号为 1 的行 */
                {8, 9, 10, 11}   /*  初始化索引号为 2 的行 */
            };
            

            List<List<int>> sendDataQueue = new List<List<int>>(6);
            if (sendDataQueue.Count > 0)
            {
                int j = sendDataQueue[0].Count;
            }
            List<int> timesList = new List<int>();

            for (int i = 0; i < 6; i++)
            {
                sendDataQueue.Add(timesList);
            }

            for (int i = 0; i < 6; i++)
            {
                sendDataQueue[i].Add(1);
            }
            

            List<byte[]> DbCmdLsit = new List<byte[]>();
            DbCmdLsit.Add(CmdSetCapTime);

            int l = DbCmdLsit[0][4];

            /* string strHour = "18,9,10,22";
             string strMinute = "15,16,52,45";
             //最多24组
             string[] HourArray = strHour.Split(new char[] { ',' });
             string[] MinuteArray = strMinute.Split(new char[] { ',' });

             List<int> timesList = new List<int>(24);

             for (int i = 0; i < HourArray.Length; i++)
             {
                 timesList.Add(Convert.ToInt32(HourArray[i]) * 60 + Convert.ToInt32(MinuteArray[i]));
             }

             timesList.Sort();//默认从小到大排序,排序完了之后分解并存放在数组中
             int hour = timesList[0] / 60;
             int minute = timesList[0]-60*hour;

             for (int i = 0, j = 9; i < 24; i++)
             {
                 if (i < HourArray.GetLength(0))
                 {
                     CmdSetCapTime[j++] = Convert.ToByte(HourArray[i]);
                     CmdSetCapTime[j++] = Convert.ToByte(MinuteArray[i]);
                 }
             }*/

        }


    }
}
