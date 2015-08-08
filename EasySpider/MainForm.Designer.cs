namespace EasySpider
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.btn_Ok = new System.Windows.Forms.Button();
            this.t_show = new System.Windows.Forms.TextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.t_surl = new System.Windows.Forms.TextBox();
            this.btn_stop = new System.Windows.Forms.Button();
            this.l_stat = new System.Windows.Forms.Label();
            this.btn_new = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btn_Ok
            // 
            this.btn_Ok.Location = new System.Drawing.Point(337, 69);
            this.btn_Ok.Name = "btn_Ok";
            this.btn_Ok.Size = new System.Drawing.Size(85, 26);
            this.btn_Ok.TabIndex = 0;
            this.btn_Ok.Text = "开始搜集";
            this.btn_Ok.UseVisualStyleBackColor = true;
            this.btn_Ok.Click += new System.EventHandler(this.btn_Ok_Click);
            // 
            // t_show
            // 
            this.t_show.Location = new System.Drawing.Point(12, 38);
            this.t_show.Multiline = true;
            this.t_show.Name = "t_show";
            this.t_show.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.t_show.Size = new System.Drawing.Size(315, 219);
            this.t_show.TabIndex = 2;
            // 
            // button1
            // 
            this.button1.DialogResult = System.Windows.Forms.DialogResult.Abort;
            this.button1.Location = new System.Drawing.Point(337, 231);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(85, 26);
            this.button1.TabIndex = 4;
            this.button1.Text = "退出";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(337, 38);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(85, 26);
            this.button2.TabIndex = 5;
            this.button2.Text = "修改种子";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // t_surl
            // 
            this.t_surl.Location = new System.Drawing.Point(116, 9);
            this.t_surl.Name = "t_surl";
            this.t_surl.Size = new System.Drawing.Size(211, 20);
            this.t_surl.TabIndex = 6;
            this.t_surl.Text = "http://blog.sina.com.cn/lm/rank/";
            // 
            // btn_stop
            // 
            this.btn_stop.Location = new System.Drawing.Point(337, 100);
            this.btn_stop.Name = "btn_stop";
            this.btn_stop.Size = new System.Drawing.Size(85, 26);
            this.btn_stop.TabIndex = 8;
            this.btn_stop.Text = "停止";
            this.btn_stop.UseVisualStyleBackColor = true;
            this.btn_stop.Click += new System.EventHandler(this.btn_stop_Click);
            // 
            // l_stat
            // 
            this.l_stat.AutoSize = true;
            this.l_stat.Location = new System.Drawing.Point(12, 12);
            this.l_stat.Name = "l_stat";
            this.l_stat.Size = new System.Drawing.Size(91, 13);
            this.l_stat.TabIndex = 9;
            this.l_stat.Text = "状态：已就绪。";
            // 
            // btn_new
            // 
            this.btn_new.Location = new System.Drawing.Point(337, 7);
            this.btn_new.Name = "btn_new";
            this.btn_new.Size = new System.Drawing.Size(85, 26);
            this.btn_new.TabIndex = 10;
            this.btn_new.Text = "初始链接";
            this.btn_new.UseVisualStyleBackColor = true;
            this.btn_new.Click += new System.EventHandler(this.btn_new_Click);
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(337, 132);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(85, 26);
            this.button3.TabIndex = 11;
            this.button3.Text = "查看结果";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(432, 269);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.btn_new);
            this.Controls.Add(this.l_stat);
            this.Controls.Add(this.btn_stop);
            this.Controls.Add(this.t_surl);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.t_show);
            this.Controls.Add(this.btn_Ok);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "MainForm";
            this.Text = "新浪博客地址搜集器";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btn_Ok;
        public  System.Windows.Forms.TextBox t_show;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.TextBox t_surl;
        private System.Windows.Forms.Button btn_stop;
        private System.Windows.Forms.Label l_stat;
        private System.Windows.Forms.Button btn_new;
        private System.Windows.Forms.Button button3;
    }
}

