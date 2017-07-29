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
            this.buttonClear = new System.Windows.Forms.Button();
            this.buttonViewClientInfo = new System.Windows.Forms.Button();
            this.richTextBox1 = new System.Windows.Forms.RichTextBox();
            this.label12 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.buttonCloseServer = new System.Windows.Forms.Button();
            this.textBoxNextTime = new System.Windows.Forms.TextBox();
            this.buttonCapNow = new System.Windows.Forms.Button();
            this.textBoxServerPort = new System.Windows.Forms.TextBox();
            this.label11 = new System.Windows.Forms.Label();
            this.textBoxServerIP = new System.Windows.Forms.TextBox();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // buttonOpenServer
            // 
            this.buttonOpenServer.Location = new System.Drawing.Point(331, 20);
            this.buttonOpenServer.Name = "buttonOpenServer";
            this.buttonOpenServer.Size = new System.Drawing.Size(49, 23);
            this.buttonOpenServer.TabIndex = 0;
            this.buttonOpenServer.Text = "打开";
            this.buttonOpenServer.UseVisualStyleBackColor = true;
            this.buttonOpenServer.Click += new System.EventHandler(this.button1_Click);
            // 
            // buttonUpload
            // 
            this.buttonUpload.Location = new System.Drawing.Point(386, 57);
            this.buttonUpload.Name = "buttonUpload";
            this.buttonUpload.Size = new System.Drawing.Size(49, 23);
            this.buttonUpload.TabIndex = 1;
            this.buttonUpload.Text = "上传";
            this.buttonUpload.UseVisualStyleBackColor = true;
            this.buttonUpload.Click += new System.EventHandler(this.button2_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.buttonClear);
            this.groupBox1.Controls.Add(this.buttonViewClientInfo);
            this.groupBox1.Controls.Add(this.richTextBox1);
            this.groupBox1.Controls.Add(this.label12);
            this.groupBox1.Controls.Add(this.label10);
            this.groupBox1.Controls.Add(this.buttonCloseServer);
            this.groupBox1.Controls.Add(this.textBoxNextTime);
            this.groupBox1.Controls.Add(this.buttonCapNow);
            this.groupBox1.Controls.Add(this.textBoxServerPort);
            this.groupBox1.Controls.Add(this.buttonUpload);
            this.groupBox1.Controls.Add(this.label11);
            this.groupBox1.Controls.Add(this.textBoxServerIP);
            this.groupBox1.Controls.Add(this.buttonOpenServer);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Location = new System.Drawing.Point(0, 0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(441, 487);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "服务器配置";
            // 
            // buttonClear
            // 
            this.buttonClear.Location = new System.Drawing.Point(132, 82);
            this.buttonClear.Name = "buttonClear";
            this.buttonClear.Size = new System.Drawing.Size(52, 23);
            this.buttonClear.TabIndex = 28;
            this.buttonClear.Text = "Clear";
            this.buttonClear.UseVisualStyleBackColor = true;
            this.buttonClear.Click += new System.EventHandler(this.buttonClear_Click);
            // 
            // buttonViewClientInfo
            // 
            this.buttonViewClientInfo.Location = new System.Drawing.Point(13, 82);
            this.buttonViewClientInfo.Name = "buttonViewClientInfo";
            this.buttonViewClientInfo.Size = new System.Drawing.Size(95, 23);
            this.buttonViewClientInfo.TabIndex = 27;
            this.buttonViewClientInfo.Text = "查询设备信息";
            this.buttonViewClientInfo.UseVisualStyleBackColor = true;
            this.buttonViewClientInfo.Click += new System.EventHandler(this.buttonViewClientInfo_Click);
            // 
            // richTextBox1
            // 
            this.richTextBox1.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.richTextBox1.Location = new System.Drawing.Point(12, 111);
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.Size = new System.Drawing.Size(423, 370);
            this.richTextBox1.TabIndex = 26;
            this.richTextBox1.Text = "";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(178, 25);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(71, 12);
            this.label12.TabIndex = 14;
            this.label12.Text = "服务器Port:";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(169, 59);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(89, 12);
            this.label10.TabIndex = 25;
            this.label10.Text = "倒计时的时间：";
            // 
            // buttonCloseServer
            // 
            this.buttonCloseServer.Location = new System.Drawing.Point(386, 20);
            this.buttonCloseServer.Name = "buttonCloseServer";
            this.buttonCloseServer.Size = new System.Drawing.Size(49, 23);
            this.buttonCloseServer.TabIndex = 13;
            this.buttonCloseServer.Text = "关闭";
            this.buttonCloseServer.UseVisualStyleBackColor = true;
            this.buttonCloseServer.Click += new System.EventHandler(this.buttonCloseServer_Click);
            // 
            // textBoxNextTime
            // 
            this.textBoxNextTime.Location = new System.Drawing.Point(264, 55);
            this.textBoxNextTime.Name = "textBoxNextTime";
            this.textBoxNextTime.Size = new System.Drawing.Size(31, 21);
            this.textBoxNextTime.TabIndex = 24;
            this.textBoxNextTime.Text = "5";
            // 
            // buttonCapNow
            // 
            this.buttonCapNow.Location = new System.Drawing.Point(331, 57);
            this.buttonCapNow.Name = "buttonCapNow";
            this.buttonCapNow.Size = new System.Drawing.Size(49, 23);
            this.buttonCapNow.TabIndex = 23;
            this.buttonCapNow.Text = "采样";
            this.buttonCapNow.UseVisualStyleBackColor = true;
            this.buttonCapNow.Click += new System.EventHandler(this.buttonCapNow_Click);
            // 
            // textBoxServerPort
            // 
            this.textBoxServerPort.Location = new System.Drawing.Point(255, 20);
            this.textBoxServerPort.Name = "textBoxServerPort";
            this.textBoxServerPort.Size = new System.Drawing.Size(40, 21);
            this.textBoxServerPort.TabIndex = 12;
            this.textBoxServerPort.Text = "8085";
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
            // textBoxServerIP
            // 
            this.textBoxServerIP.Location = new System.Drawing.Point(69, 20);
            this.textBoxServerIP.Name = "textBoxServerIP";
            this.textBoxServerIP.Size = new System.Drawing.Size(103, 21);
            this.textBoxServerIP.TabIndex = 1;
            this.textBoxServerIP.Text = "192.168.3.83";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(441, 487);
            this.Controls.Add(this.groupBox1);
            this.Name = "MainForm";
            this.Text = "漏水采集平台";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button buttonOpenServer;
        private System.Windows.Forms.Button buttonUpload;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.TextBox textBoxNextTime;
        private System.Windows.Forms.Button buttonCapNow;
        private System.Windows.Forms.TextBox textBoxServerIP;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Button buttonCloseServer;
        private System.Windows.Forms.TextBox textBoxServerPort;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Button buttonViewClientInfo;
        private System.Windows.Forms.RichTextBox richTextBox1;
        private System.Windows.Forms.Button buttonClear;
    }
}

