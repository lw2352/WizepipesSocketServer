namespace WizepipesSocketServer
{
    partial class MainForm
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.buttonOpenServer = new System.Windows.Forms.Button();
            this.buttonUpload = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.textBoxServerIP = new System.Windows.Forms.TextBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.checkBoxAutoTest = new System.Windows.Forms.CheckBox();
            this.label10 = new System.Windows.Forms.Label();
            this.textBoxNextTime = new System.Windows.Forms.TextBox();
            this.buttonCapNow = new System.Windows.Forms.Button();
            this.label9 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.buttonReadCurrentOpenAndCloseTime = new System.Windows.Forms.Button();
            this.buttonSetCurrentOpenAndCloseTime = new System.Windows.Forms.Button();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.buttonReadOpenAndCloseTime = new System.Windows.Forms.Button();
            this.buttonSetOpenAndCloseTime = new System.Windows.Forms.Button();
            this.textBoxCloseTime = new System.Windows.Forms.TextBox();
            this.textBoxOpenTime = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.buttonReadCapTime = new System.Windows.Forms.Button();
            this.buttonSetCapTime = new System.Windows.Forms.Button();
            this.textBoxMinute = new System.Windows.Forms.TextBox();
            this.textBoxHour = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.textBoxServerPort = new System.Windows.Forms.TextBox();
            this.buttonCloseServer = new System.Windows.Forms.Button();
            this.label12 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // buttonOpenServer
            // 
            this.buttonOpenServer.Location = new System.Drawing.Point(178, 20);
            this.buttonOpenServer.Name = "buttonOpenServer";
            this.buttonOpenServer.Size = new System.Drawing.Size(49, 23);
            this.buttonOpenServer.TabIndex = 0;
            this.buttonOpenServer.Text = "打开";
            this.buttonOpenServer.UseVisualStyleBackColor = true;
            this.buttonOpenServer.Click += new System.EventHandler(this.button1_Click);
            // 
            // buttonUpload
            // 
            this.buttonUpload.Location = new System.Drawing.Point(268, 20);
            this.buttonUpload.Name = "buttonUpload";
            this.buttonUpload.Size = new System.Drawing.Size(54, 23);
            this.buttonUpload.TabIndex = 1;
            this.buttonUpload.Text = "上传";
            this.buttonUpload.UseVisualStyleBackColor = true;
            this.buttonUpload.Click += new System.EventHandler(this.button2_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.groupBox1.Controls.Add(this.label12);
            this.groupBox1.Controls.Add(this.buttonCloseServer);
            this.groupBox1.Controls.Add(this.textBoxServerPort);
            this.groupBox1.Controls.Add(this.label11);
            this.groupBox1.Controls.Add(this.textBoxServerIP);
            this.groupBox1.Controls.Add(this.buttonOpenServer);
            this.groupBox1.Location = new System.Drawing.Point(0, 0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(227, 641);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "服务器配置";
            // 
            // textBoxServerIP
            // 
            this.textBoxServerIP.Location = new System.Drawing.Point(69, 20);
            this.textBoxServerIP.Name = "textBoxServerIP";
            this.textBoxServerIP.Size = new System.Drawing.Size(103, 21);
            this.textBoxServerIP.TabIndex = 1;
            this.textBoxServerIP.Text = "192.168.3.83";
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.groupBox2.Controls.Add(this.checkBoxAutoTest);
            this.groupBox2.Controls.Add(this.label10);
            this.groupBox2.Controls.Add(this.textBoxNextTime);
            this.groupBox2.Controls.Add(this.buttonCapNow);
            this.groupBox2.Controls.Add(this.label9);
            this.groupBox2.Controls.Add(this.label8);
            this.groupBox2.Controls.Add(this.label7);
            this.groupBox2.Controls.Add(this.label6);
            this.groupBox2.Controls.Add(this.buttonReadCurrentOpenAndCloseTime);
            this.groupBox2.Controls.Add(this.buttonSetCurrentOpenAndCloseTime);
            this.groupBox2.Controls.Add(this.textBox2);
            this.groupBox2.Controls.Add(this.textBox1);
            this.groupBox2.Controls.Add(this.label5);
            this.groupBox2.Controls.Add(this.buttonReadOpenAndCloseTime);
            this.groupBox2.Controls.Add(this.buttonSetOpenAndCloseTime);
            this.groupBox2.Controls.Add(this.textBoxCloseTime);
            this.groupBox2.Controls.Add(this.textBoxOpenTime);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Controls.Add(this.buttonReadCapTime);
            this.groupBox2.Controls.Add(this.buttonSetCapTime);
            this.groupBox2.Controls.Add(this.textBoxMinute);
            this.groupBox2.Controls.Add(this.textBoxHour);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Controls.Add(this.buttonUpload);
            this.groupBox2.Location = new System.Drawing.Point(227, 0);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(330, 641);
            this.groupBox2.TabIndex = 3;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "设备配置";
            // 
            // checkBoxAutoTest
            // 
            this.checkBoxAutoTest.AutoSize = true;
            this.checkBoxAutoTest.Location = new System.Drawing.Point(10, 25);
            this.checkBoxAutoTest.Name = "checkBoxAutoTest";
            this.checkBoxAutoTest.Size = new System.Drawing.Size(72, 16);
            this.checkBoxAutoTest.TabIndex = 27;
            this.checkBoxAutoTest.Text = "自动测试";
            this.checkBoxAutoTest.UseVisualStyleBackColor = true;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(91, 25);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(65, 12);
            this.label10.TabIndex = 25;
            this.label10.Text = "间隔时间：";
            // 
            // textBoxNextTime
            // 
            this.textBoxNextTime.Location = new System.Drawing.Point(162, 20);
            this.textBoxNextTime.Name = "textBoxNextTime";
            this.textBoxNextTime.Size = new System.Drawing.Size(31, 21);
            this.textBoxNextTime.TabIndex = 24;
            this.textBoxNextTime.Text = "5";
            // 
            // buttonCapNow
            // 
            this.buttonCapNow.Location = new System.Drawing.Point(199, 20);
            this.buttonCapNow.Name = "buttonCapNow";
            this.buttonCapNow.Size = new System.Drawing.Size(54, 23);
            this.buttonCapNow.TabIndex = 23;
            this.buttonCapNow.Text = "采样";
            this.buttonCapNow.UseVisualStyleBackColor = true;
            this.buttonCapNow.Click += new System.EventHandler(this.buttonCapNow_Click);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(8, 271);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(65, 12);
            this.label9.TabIndex = 22;
            this.label9.Text = "设置Port：";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(8, 246);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(53, 12);
            this.label8.TabIndex = 21;
            this.label8.Text = "设置IP：";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(8, 218);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(77, 12);
            this.label7.TabIndex = 20;
            this.label7.Text = "设置AP密码：";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(8, 189);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(77, 12);
            this.label6.TabIndex = 19;
            this.label6.Text = "设置AP名称：";
            // 
            // buttonReadCurrentOpenAndCloseTime
            // 
            this.buttonReadCurrentOpenAndCloseTime.Location = new System.Drawing.Point(268, 149);
            this.buttonReadCurrentOpenAndCloseTime.Name = "buttonReadCurrentOpenAndCloseTime";
            this.buttonReadCurrentOpenAndCloseTime.Size = new System.Drawing.Size(54, 23);
            this.buttonReadCurrentOpenAndCloseTime.TabIndex = 18;
            this.buttonReadCurrentOpenAndCloseTime.Text = "读取";
            this.buttonReadCurrentOpenAndCloseTime.UseVisualStyleBackColor = true;
            // 
            // buttonSetCurrentOpenAndCloseTime
            // 
            this.buttonSetCurrentOpenAndCloseTime.Location = new System.Drawing.Point(208, 149);
            this.buttonSetCurrentOpenAndCloseTime.Name = "buttonSetCurrentOpenAndCloseTime";
            this.buttonSetCurrentOpenAndCloseTime.Size = new System.Drawing.Size(54, 23);
            this.buttonSetCurrentOpenAndCloseTime.TabIndex = 17;
            this.buttonSetCurrentOpenAndCloseTime.Text = "设置";
            this.buttonSetCurrentOpenAndCloseTime.UseVisualStyleBackColor = true;
            // 
            // textBox2
            // 
            this.textBox2.Location = new System.Drawing.Point(150, 151);
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(51, 21);
            this.textBox2.TabIndex = 16;
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(93, 151);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(51, 21);
            this.textBox1.TabIndex = 15;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(8, 154);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(95, 12);
            this.label5.TabIndex = 14;
            this.label5.Text = "当前开/关时长：";
            // 
            // buttonReadOpenAndCloseTime
            // 
            this.buttonReadOpenAndCloseTime.Location = new System.Drawing.Point(268, 112);
            this.buttonReadOpenAndCloseTime.Name = "buttonReadOpenAndCloseTime";
            this.buttonReadOpenAndCloseTime.Size = new System.Drawing.Size(54, 23);
            this.buttonReadOpenAndCloseTime.TabIndex = 13;
            this.buttonReadOpenAndCloseTime.Text = "读取";
            this.buttonReadOpenAndCloseTime.UseVisualStyleBackColor = true;
            // 
            // buttonSetOpenAndCloseTime
            // 
            this.buttonSetOpenAndCloseTime.Location = new System.Drawing.Point(199, 112);
            this.buttonSetOpenAndCloseTime.Name = "buttonSetOpenAndCloseTime";
            this.buttonSetOpenAndCloseTime.Size = new System.Drawing.Size(54, 23);
            this.buttonSetOpenAndCloseTime.TabIndex = 12;
            this.buttonSetOpenAndCloseTime.Text = "设置";
            this.buttonSetOpenAndCloseTime.UseVisualStyleBackColor = true;
            // 
            // textBoxCloseTime
            // 
            this.textBoxCloseTime.Location = new System.Drawing.Point(135, 114);
            this.textBoxCloseTime.Name = "textBoxCloseTime";
            this.textBoxCloseTime.Size = new System.Drawing.Size(51, 21);
            this.textBoxCloseTime.TabIndex = 11;
            // 
            // textBoxOpenTime
            // 
            this.textBoxOpenTime.Location = new System.Drawing.Point(78, 114);
            this.textBoxOpenTime.Name = "textBoxOpenTime";
            this.textBoxOpenTime.Size = new System.Drawing.Size(51, 21);
            this.textBoxOpenTime.TabIndex = 10;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(8, 117);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(71, 12);
            this.label4.TabIndex = 9;
            this.label4.Text = "开/关时长：";
            // 
            // buttonReadCapTime
            // 
            this.buttonReadCapTime.Location = new System.Drawing.Point(268, 72);
            this.buttonReadCapTime.Name = "buttonReadCapTime";
            this.buttonReadCapTime.Size = new System.Drawing.Size(54, 23);
            this.buttonReadCapTime.TabIndex = 8;
            this.buttonReadCapTime.Text = "读取";
            this.buttonReadCapTime.UseVisualStyleBackColor = true;
            // 
            // buttonSetCapTime
            // 
            this.buttonSetCapTime.Location = new System.Drawing.Point(199, 72);
            this.buttonSetCapTime.Name = "buttonSetCapTime";
            this.buttonSetCapTime.Size = new System.Drawing.Size(54, 23);
            this.buttonSetCapTime.TabIndex = 7;
            this.buttonSetCapTime.Text = "设置";
            this.buttonSetCapTime.UseVisualStyleBackColor = true;
            // 
            // textBoxMinute
            // 
            this.textBoxMinute.Location = new System.Drawing.Point(135, 74);
            this.textBoxMinute.Name = "textBoxMinute";
            this.textBoxMinute.Size = new System.Drawing.Size(51, 21);
            this.textBoxMinute.TabIndex = 6;
            // 
            // textBoxHour
            // 
            this.textBoxHour.Location = new System.Drawing.Point(79, 74);
            this.textBoxHour.Name = "textBoxHour";
            this.textBoxHour.Size = new System.Drawing.Size(51, 21);
            this.textBoxHour.TabIndex = 5;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(146, 58);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(29, 12);
            this.label3.TabIndex = 4;
            this.label3.Text = "分钟";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(91, 58);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(29, 12);
            this.label2.TabIndex = 3;
            this.label2.Text = "小时";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(8, 77);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(65, 12);
            this.label1.TabIndex = 2;
            this.label1.Text = "采样时间：";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(4, 26);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(59, 12);
            this.label11.TabIndex = 10;
            this.label11.Text = "服务器IP:";
            // 
            // textBoxServerPort
            // 
            this.textBoxServerPort.Location = new System.Drawing.Point(81, 47);
            this.textBoxServerPort.Name = "textBoxServerPort";
            this.textBoxServerPort.Size = new System.Drawing.Size(91, 21);
            this.textBoxServerPort.TabIndex = 12;
            this.textBoxServerPort.Text = "8080";
            // 
            // buttonCloseServer
            // 
            this.buttonCloseServer.Location = new System.Drawing.Point(178, 47);
            this.buttonCloseServer.Name = "buttonCloseServer";
            this.buttonCloseServer.Size = new System.Drawing.Size(49, 23);
            this.buttonCloseServer.TabIndex = 13;
            this.buttonCloseServer.Text = "关闭";
            this.buttonCloseServer.UseVisualStyleBackColor = true;
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(4, 52);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(71, 12);
            this.label12.TabIndex = 14;
            this.label12.Text = "服务器Port:";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(561, 641);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Name = "MainForm";
            this.Text = "漏水采集平台";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button buttonOpenServer;
        private System.Windows.Forms.Button buttonUpload;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Button buttonReadCurrentOpenAndCloseTime;
        private System.Windows.Forms.Button buttonSetCurrentOpenAndCloseTime;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button buttonReadOpenAndCloseTime;
        private System.Windows.Forms.Button buttonSetOpenAndCloseTime;
        private System.Windows.Forms.TextBox textBoxCloseTime;
        private System.Windows.Forms.TextBox textBoxOpenTime;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button buttonReadCapTime;
        private System.Windows.Forms.Button buttonSetCapTime;
        private System.Windows.Forms.TextBox textBoxMinute;
        private System.Windows.Forms.TextBox textBoxHour;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.CheckBox checkBoxAutoTest;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.TextBox textBoxNextTime;
        private System.Windows.Forms.Button buttonCapNow;
        private System.Windows.Forms.TextBox textBoxServerIP;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Button buttonCloseServer;
        private System.Windows.Forms.TextBox textBoxServerPort;
        private System.Windows.Forms.Label label12;
    }
}

