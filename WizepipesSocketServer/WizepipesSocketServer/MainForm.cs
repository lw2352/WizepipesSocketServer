using System;
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
        //TODO:1.读取配置文件中的参数 2.做成windows服务 3.读取数据库命令

        SocketServer server = new SocketServer();
        public MainForm()
        {
            InitializeComponent();
            server.InitServer();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            
            server.OpenServer(textBoxServerIP.Text, Convert.ToInt32(textBoxServerPort.Text));
        }

        private void button2_Click(object sender, EventArgs e)
        {
            server.UploadData();
        }

        private void buttonCapNow_Click(object sender, EventArgs e)
        {
            int NextTime = Convert.ToInt32(textBoxNextTime.Text);

            server.SetCapTime(NextTime);
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
    }
}
