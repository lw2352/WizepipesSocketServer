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
            NetDb.readsensorcfg(1);
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

            int[,] a = new int[3, 4] {
                {0, 1, 2, 3} ,   /*  初始化索引号为 0 的行 */
                {4, 5, 6, 7} ,   /*  初始化索引号为 1 的行 */
                {8, 9, 10, 11}   /*  初始化索引号为 2 的行 */
            };

            int i=a.GetLength(0);//行数
            int j = a.GetLength(1);//列数
            
        }


    }
}
