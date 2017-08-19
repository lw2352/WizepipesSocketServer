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
            this.buttonAnalyze = new System.Windows.Forms.Button();
            this.checkBoxAutoTest = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.buttonClear = new System.Windows.Forms.Button();
            this.buttonViewClientInfo = new System.Windows.Forms.Button();
            this.buttonCloseServer = new System.Windows.Forms.Button();
            this.buttonCapNow = new System.Windows.Forms.Button();
            this.richTextBox1 = new System.Windows.Forms.RichTextBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // buttonOpenServer
            // 
            this.buttonOpenServer.Location = new System.Drawing.Point(129, 20);
            this.buttonOpenServer.Name = "buttonOpenServer";
            this.buttonOpenServer.Size = new System.Drawing.Size(49, 23);
            this.buttonOpenServer.TabIndex = 0;
            this.buttonOpenServer.Text = "打开";
            this.buttonOpenServer.UseVisualStyleBackColor = true;
            this.buttonOpenServer.Click += new System.EventHandler(this.button1_Click);
            // 
            // buttonUpload
            // 
            this.buttonUpload.Location = new System.Drawing.Point(184, 49);
            this.buttonUpload.Name = "buttonUpload";
            this.buttonUpload.Size = new System.Drawing.Size(49, 23);
            this.buttonUpload.TabIndex = 1;
            this.buttonUpload.Text = "上传";
            this.buttonUpload.UseVisualStyleBackColor = true;
            this.buttonUpload.Click += new System.EventHandler(this.button2_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.buttonAnalyze);
            this.groupBox1.Controls.Add(this.checkBoxAutoTest);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.buttonClear);
            this.groupBox1.Controls.Add(this.buttonViewClientInfo);
            this.groupBox1.Controls.Add(this.buttonCloseServer);
            this.groupBox1.Controls.Add(this.buttonCapNow);
            this.groupBox1.Controls.Add(this.buttonUpload);
            this.groupBox1.Controls.Add(this.buttonOpenServer);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupBox1.Location = new System.Drawing.Point(0, 0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(800, 186);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "服务器配置";
            // 
            // buttonAnalyze
            // 
            this.buttonAnalyze.Location = new System.Drawing.Point(129, 106);
            this.buttonAnalyze.Name = "buttonAnalyze";
            this.buttonAnalyze.Size = new System.Drawing.Size(75, 23);
            this.buttonAnalyze.TabIndex = 32;
            this.buttonAnalyze.Text = "myTest";
            this.buttonAnalyze.UseVisualStyleBackColor = true;
            this.buttonAnalyze.Click += new System.EventHandler(this.buttonAnalyze_Click);
            // 
            // checkBoxAutoTest
            // 
            this.checkBoxAutoTest.AutoSize = true;
            this.checkBoxAutoTest.Location = new System.Drawing.Point(27, 53);
            this.checkBoxAutoTest.Name = "checkBoxAutoTest";
            this.checkBoxAutoTest.Size = new System.Drawing.Size(72, 16);
            this.checkBoxAutoTest.TabIndex = 31;
            this.checkBoxAutoTest.Text = "自动测试";
            this.checkBoxAutoTest.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 25);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(107, 12);
            this.label1.TabIndex = 29;
            this.label1.Text = "打开或关闭服务器:";
            // 
            // buttonClear
            // 
            this.buttonClear.Location = new System.Drawing.Point(129, 157);
            this.buttonClear.Name = "buttonClear";
            this.buttonClear.Size = new System.Drawing.Size(52, 23);
            this.buttonClear.TabIndex = 28;
            this.buttonClear.Text = "清空";
            this.buttonClear.UseVisualStyleBackColor = true;
            this.buttonClear.Click += new System.EventHandler(this.buttonClear_Click);
            // 
            // buttonViewClientInfo
            // 
            this.buttonViewClientInfo.Location = new System.Drawing.Point(10, 157);
            this.buttonViewClientInfo.Name = "buttonViewClientInfo";
            this.buttonViewClientInfo.Size = new System.Drawing.Size(95, 23);
            this.buttonViewClientInfo.TabIndex = 27;
            this.buttonViewClientInfo.Text = "查询设备信息";
            this.buttonViewClientInfo.UseVisualStyleBackColor = true;
            this.buttonViewClientInfo.Click += new System.EventHandler(this.buttonViewClientInfo_Click);
            // 
            // buttonCloseServer
            // 
            this.buttonCloseServer.Location = new System.Drawing.Point(184, 20);
            this.buttonCloseServer.Name = "buttonCloseServer";
            this.buttonCloseServer.Size = new System.Drawing.Size(49, 23);
            this.buttonCloseServer.TabIndex = 13;
            this.buttonCloseServer.Text = "关闭";
            this.buttonCloseServer.UseVisualStyleBackColor = true;
            this.buttonCloseServer.Click += new System.EventHandler(this.buttonCloseServer_Click);
            // 
            // buttonCapNow
            // 
            this.buttonCapNow.Location = new System.Drawing.Point(129, 49);
            this.buttonCapNow.Name = "buttonCapNow";
            this.buttonCapNow.Size = new System.Drawing.Size(49, 23);
            this.buttonCapNow.TabIndex = 23;
            this.buttonCapNow.Text = "采样";
            this.buttonCapNow.UseVisualStyleBackColor = true;
            this.buttonCapNow.Click += new System.EventHandler(this.buttonCapNow_Click);
            // 
            // richTextBox1
            // 
            this.richTextBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.richTextBox1.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.richTextBox1.Location = new System.Drawing.Point(3, 17);
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.Size = new System.Drawing.Size(794, 502);
            this.richTextBox1.TabIndex = 26;
            this.richTextBox1.Text = "";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.richTextBox1);
            this.groupBox2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox2.Location = new System.Drawing.Point(0, 186);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(800, 522);
            this.groupBox2.TabIndex = 29;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "数据显示";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 708);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Name = "MainForm";
            this.Text = "漏水采集平台";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button buttonOpenServer;
        private System.Windows.Forms.Button buttonUpload;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button buttonCapNow;
        private System.Windows.Forms.Button buttonCloseServer;
        private System.Windows.Forms.Button buttonViewClientInfo;
        private System.Windows.Forms.RichTextBox richTextBox1;
        private System.Windows.Forms.Button buttonClear;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox checkBoxAutoTest;
        private System.Windows.Forms.Button buttonAnalyze;
    }
}

