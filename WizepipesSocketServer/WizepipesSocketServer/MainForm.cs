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
        SocketServer server = new SocketServer();
        public MainForm()
        {
            InitializeComponent();
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
            byte[] cmd = new byte[] { 0xA5, 0xA5, 0x25, 0xFF, 0xFF, 0xFF, 0xFF, 0x01, 0x04, 0x0A, 0x1E, 0x00, 0x00, 0xFF, 0x5A, 0x5A };
            int NextTime = Convert.ToInt32(textBoxNextTime.Text);
            if (DateTime.Now.Minute + NextTime <= 59)
            {
                cmd[9] = (byte)DateTime.Now.Hour;
                cmd[10] = (byte)(DateTime.Now.Minute + NextTime);//当前时刻加5分钟
            }
            else
            { //分钟数大于60
                cmd[9] = (byte)(DateTime.Now.Hour + 1);
                cmd[10] = (byte)(DateTime.Now.Minute + NextTime - 60);
            }
            server.AddCmdToQueue(0xFF, cmd);
        }
    }
}
