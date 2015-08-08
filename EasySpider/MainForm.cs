using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Threading;
using System.Diagnostics;

namespace EasySpider
{
    public partial class MainForm : Form
    {
        object olock_bl = new object();
        object olock_ini = new object();
        public bool key_bl = true;
        public bool key_ini = true;

        //每txt存最大地址数，当然（也就是）是不重复的地址数
        public int _maxUrls = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["MaxUrlsPerFile"]);

        //要用到的文件的定义
        public string path_blacklist = System.Environment.CurrentDirectory + "\\" + "blacklist.txt";
        public string path_blacklistbk = System.Environment.CurrentDirectory + "\\" + "blacklistbk.txt";
        public string path_ini = System.Environment.CurrentDirectory + "\\" + "ini.txt";
        public string path_ibk = System.Environment.CurrentDirectory + "\\" + "ibk.txt";
        public System.IO.FileInfo fi_blacklist = new FileInfo("blacklist.txt");
        public System.IO.FileInfo fi_blacklistbk = new FileInfo("blacklistbk.txt");
        public System.IO.FileInfo fi_ini = new FileInfo("ini.txt");
        public System.IO.FileInfo fi_ibk = new FileInfo("ibk.txt");

        ToolTip tt = new ToolTip();
        public MainForm()
        {
            InitializeComponent();
            l_stat.Text = "状态：已就绪。";
            t_show.Text += "软件过期时间：2013年5月11日。\r\n";
            blackListBackup(); //将blacklist.txt备份成blacklistbk.txt
            
            //将blacklist【txt】读入【list】,滤掉重复
            bl2li = new Thread(new ThreadStart(blackTxt2List));
            bl2li.Start();
            bl2li.Join();
            
            //将blacklist无重复的【list】写回【txt】
            li2bl = new Thread(new ThreadStart(blackList2Txt));
            li2bl.Start();
            li2bl.Join();

            blackListRecover(); //如果blacklist.txt小了、空了，则还原blacklist.txt

            //自动开始搜集
            MainForm.surl = t_surl.Text;
            done = false;
            l_stat.Text = "状态：正在搜集...";
            enabledButtons(false);
            th = new Thread(new ThreadStart(run));
            th.Start();

            ////每隔时间段写 ini.txt 备份 ibk.txt
            //System.Timers.Timer timeCollect;
            //timeCollect = new System.Timers.Timer(1000);
            //timeCollect.Elapsed += new System.Timers.ElapsedEventHandler(writeIni);
            //timeCollect.AutoReset = true;
            //timeCollect.Enabled = true;

            //每隔时间段测试链接有效
            t2 = new System.Timers.Timer(15 * 1000);
            t2.Elapsed += new System.Timers.ElapsedEventHandler(checkUrl);
            t2.AutoReset = true;
            t2.Enabled = true;

            //每隔时间段清理内存
            t4 = new System.Timers.Timer(30 * 60 * 1000);
            t4.Elapsed += new System.Timers.ElapsedEventHandler(clearMemory);
            t4.AutoReset = true;
            t4.Enabled = true;

            if (File.Exists(path_ini) && File.Exists(path_ibk) && fi_ini.Length < 10 && fi_ibk.Length > 10)
            {
                String sourcePath = path_ibk;
                String targetPath = path_ini;
                System.IO.File.Copy(sourcePath, targetPath, true);
            }

            if (File.Exists(path_ini) && fi_ini.Length > 10)
            {
                while(true)
                {
                    if (!key_ini)
                        System.Threading.Thread.Sleep(100);
                    else
                    {
                        key_ini = false;
                        lock (olock_ini)
                        {
                            Stream str = fi_ini.Open(FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                            StreamReader sr = new StreamReader(str);

                            for (int i = 0; i < 40; i++)
                            {
                                string tmp = sr.ReadLine() + "";
                                if (tmp.Length > 30)
                                {
                                    MainForm.addSeed(tmp);
                                }
                            }
                            if (seed.Count < 2)
                                t_surl.Text = "http://blog.sina.com.cn/lm/rank/";
                            else
                                t_surl.Text = seed.ElementAt(0);
                            sr.Close();
                            str.Close();
                        }
                        key_ini = true;
                        break;
                    }
                }
            }
            else
                t_surl.Text = "http://blog.sina.com.cn/lm/rank/";
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            this.tt.SetToolTip(this.l_stat, "每搜集1万个地址，程序会自动为其生成一个txt文件。\r\n每1个单独文件中不会有重复地址。");
            this.tt.SetToolTip(this.btn_new, "修改用于起始抓取的地址（从ini.txt中的20个种子地址中选取下一个种子）。\r\n由于种子文件会自动更新，这个功能应该很少会用到吧！");
            this.tt.SetToolTip(this.button2, "如果是首次使用本软件或地址数量很长时间没有增长，则需要修改ini.txt文件（种子文件）。\r\n建议最好到网上随机选取20个毫不相干的博客地址，而不要在已搜集好的地址中选取。\r\n种子文件平时是不需要修改的。");
            this.tt.SetToolTip(this.btn_stop, "停止搜集。停止的过程可能要耗费一点时间。");
            this.tt.SetToolTip(this.t_surl, "键盘输入指定起始种子，一般不会用到的。");
            this.tt.SetToolTip(this.t_show, "文件生成的消息会在这里显示。");
            this.tt.SetToolTip(this.btn_Ok, "继续上一次的搜集。");
            this.tt.SetToolTip(this.button1, "关闭时种子信息会被更新和保存，不必担心下一次启动会有很多重复地址。");
            this.tt.SetToolTip(this.button3, "打开生成的txt文件所处于的文件夹。");
        }

        public static int i = 0;
        string file_out;
        Boolean done = true;
        System.Timers.Timer t2;
        System.Timers.Timer t4;
        public static List<string> list = new List<string>();
        public static List<string> seed = new List<string>();
        public static List<string> blacklist = new List<string>();
        //public static List<string> checksame = new List<string>();
        public static Queue<string> queuesame = new Queue<string>();
        public static int iseed = 0, giseed = 0, isame = 0, blacknum = 0;
        public static int isUrlGood = 0;
        public static int urlcount = 0;
        public bool blackopen = true;
        public bool iniopen = true;

        public static string surl = "";
        string time;
        private Client myclient = new Client();

        public static void addSeed(string str)
        {
            if (seed.Count >= 20 && !seed.Contains(str) && !blacklist.Contains(str) && str.Length > 30)
            {
                seed.Insert(iseed, str);
                iseed++;
                iseed = iseed % 20;
                //SetStat(iseed.ToString());
            }
            else if (seed.Count < 20 && !seed.Contains(str) && !blacklist.Contains(str) && str.Length > 30)
            {
                seed.Add(str);
            }
        }

        public static void addSame(string str)
        {
            //checksame.Add(str);
            if (queuesame.Count < 10000)
                queuesame.Enqueue(str);
            else
            {
                queuesame.Dequeue();
                queuesame.Enqueue(str);
            }
            //如果重复url个数超过10个，则：
            if (sameNum(str) > 10)
            {
                if(!blacklist.Contains(str))
                    blacklist.Add(str);
                if (seed.Contains(str))
                    seed.Remove(str);
            }
        }

        public static int sameNum(string str)
        {
            int samenum = 0;
            foreach (string s in queuesame)
            {
                if (str == s)
                    samenum++;
            }
            return samenum;
        }

        private string getSeed()
        {
            string get;
            if (seed.Count < 2)
                get = "http://blog.sina.com.cn/lm/rank/";
            else
            {
                giseed = giseed % seed.Count;
                get = seed.ElementAt(giseed);
            }
            giseed++;
            giseed = giseed % seed.Count;
            return get;
        }

        private void run()
        {
            this.myclient.Start_ClientServer();
            while (!done)
            {
                if (MainForm.list.Count >= _maxUrls)
                {
                    MainForm.urlcount = 0;
                    time = System.DateTime.Now.Month.ToString() + "." + System.DateTime.Now.Day.ToString() + "." + System.DateTime.Now.Hour.ToString() + "." + System.DateTime.Now.Minute.ToString() + "." + System.DateTime.Now.Second.ToString();
                    file_out = "url_" + time + "_" + i.ToString() + ".html";
                    FileInfo fi_out = new FileInfo("txts\\" + file_out);
                    Stream str_out = null;
                    if (File.Exists(System.Environment.CurrentDirectory + "\\txts\\" + file_out))
                        str_out = fi_out.Open(FileMode.Append, FileAccess.Write, FileShare.ReadWrite);
                    else
                        str_out = fi_out.Open(FileMode.CreateNew, FileAccess.Write, FileShare.ReadWrite);
                    StreamWriter sw_out;
                    sw_out = new StreamWriter(str_out);
                    string[] getall = MainForm.list.ToArray();
                    MainForm.list.Clear();
                    foreach (string s in getall)
                    {
                        MainForm.addSeed(s);
                        sw_out.WriteLine(s);
                    }
                    sw_out.Close();
                    str_out.Close();
                    SetText("文件 " + file_out + " 已生成！");
                    i++;
                }
                if (blacklist.Count > 0)
                {
                    blackListBackup(); //将blacklist.txt备份成blacklistbk.txt
                    
                    //将blacklist【txt】读入【list】,滤掉重复
                    bl2li = new Thread(new ThreadStart(blackTxt2List));
                    bl2li.Start();
                    bl2li.Join();

                    //将blacklist无重复的【list】写回【txt】
                    li2bl = new Thread(new ThreadStart(blackList2Txt));
                    li2bl.Start();
                    li2bl.Join();

                    //如果blacklist.txt小了、空了，则还原blacklist.txt
                    blackListRecover(); 
                }
                if (isUrlGood > 2)
                {
                    wini = new Thread(new ThreadStart(writeIni));
                    wini.Start();
                    wini.Join();
                }
                System.Threading.Thread.Sleep(500);
                SetStat("已搜集：" + urlcount.ToString()+"个...");
            }
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.myclient.Stop_ClientServer();  
        }

        //修改种子文件
        private void button2_Click(object sender, EventArgs e)
        {
            if (done == true)
            {
                if (File.Exists(path_ini))
                    System.Diagnostics.Process.Start(path_ini);
                else
                {
                    if (!key_ini)
                        MessageBox.Show("种子文件正在被读写，请1秒后再试。");
                    else
                    {
                        key_ini = false;
                        lock (olock_ini)
                        {
                            Stream str_run = fi_ini.Open(FileMode.CreateNew, FileAccess.Write, FileShare.ReadWrite);
                            str_run.Close();
                            System.Diagnostics.Process.Start(path_ini);
                        }
                        key_ini = true;
                    }
                }
            }
            else
                MessageBox.Show("请先停止程序！");
        }

        //退出键
        private void button1_Click(object sender, EventArgs e)
        {
            if (done == true)
            {
                try
                {
                    Process.GetCurrentProcess().Kill();
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            else
                MessageBox.Show("请先停止程序！");
        }

        private delegate void SetTextCallback(string str);
        private void SetText(string text)
        {
            if (this.t_show.InvokeRequired)
            {
                SetTextCallback d = new SetTextCallback(SetText);
                this.Invoke(d, new object[] { text });
            }
            else
            {
                //this.t_surl.Text = text;
                this.t_show.AppendText(text);
                this.t_show.Text += "\r\n";
            }
        }

        private void AddStat(string text)
        {
            if (this.t_show.InvokeRequired)
            {
                SetTextCallback d3 = new SetTextCallback(AddStat);
                this.Invoke(d3, new object[] { text });
            }
            else
            {
                t_show.Text += text + "\r\n";
            }
        }

        private void SetStat(string text)
        {
            if (this.t_show.InvokeRequired)
            {
                SetTextCallback d2 = new SetTextCallback(SetStat);
                this.Invoke(d2, new object[] { text });
            }
            else
            {
                l_stat.Text = text;
            }
        }

        private void enabledButtons(bool en)
        {
            btn_new.Enabled = en;
            button2.Enabled = en;
            btn_Ok.Enabled = en;
            button1.Enabled = en;
            //btn_stop.Enabled = !en;
        }

        Thread th;
        //开始搜集
        private void btn_Ok_Click(object sender, EventArgs e)
        {
            if (done == false)
                MessageBox.Show("程序已在运行！");
            else
            {
                MainForm.surl = t_surl.Text;
                done = false;
                t2.Enabled = true;
                l_stat.Text = "状态：正在搜集...";
                enabledButtons(false);
                th = new Thread(new ThreadStart(run));
                th.Start();
            }
        }

        Thread wini;
        /*
        wini = new Thread(new ThreadStart(writeIni));
        wini.Start();
        wini.Join();
         */
        private void writeIni()
        {
            while(true)
            {
                if (!key_ini)
                    System.Threading.Thread.Sleep(100);
                else
                {
                    key_ini = false;
                    lock (olock_ini)
                    {
                        Stream str_ini = fi_ini.Open(FileMode.Create, FileAccess.Write, FileShare.ReadWrite);
                        StreamWriter sw_ini = new StreamWriter(str_ini);
                        for (int i = 0; i < 20 && i < seed.Count; i++)
                        {
                            string str = this.getSeed();
                            sw_ini.WriteLine(str);
                        }
                        sw_ini.Close();
                        str_ini.Close();
                        if (File.Exists(path_ini) && fi_ini.Length > 10)
                        {
                            String sourcePath = path_ini;
                            String targetPath = path_ibk;
                            System.IO.File.Copy(sourcePath, targetPath, true);
                        }
                    }
                    key_ini = true;
                    break;
                }
            }
        }

        //将blacklist无重复的【list】写入【txt】
        Thread li2bl;
        /*
        li2bl = new Thread(new ThreadStart(blackList2Txt));
        li2bl.Start();
        li2bl.Join();
        */
        private void blackList2Txt()
        {   //避免blacklist.txt过大的办法：将txt读入list(其间排除重复)，再整个写回txt
            System.Threading.Thread.Sleep(300);
            while(true)
            {
                if (!key_bl)
                    System.Threading.Thread.Sleep(100);
                else
                {
                    key_bl = false;
                    lock (olock_bl)
                    {
                        Stream str_blist = fi_blacklist.Open(FileMode.Create, FileAccess.Write, FileShare.ReadWrite);
                        StreamWriter sw_blist = new StreamWriter(str_blist);
                        if (blacklist.Count != 0)
                        {
                            List<string> tmplist = new List<string>(blacklist);
                            foreach (string s in tmplist)
                            {
                                sw_blist.WriteLine(s);
                            }
                            tmplist.Clear();
                            sw_blist.Close();
                            str_blist.Close();
                        }
                    }
                    key_bl = true;
                    break;
                }
            }
        }

        //将blacklist【txt】备份
        private void blackListBackup()
        {
            if (File.Exists(path_blacklist) && File.Exists(path_blacklistbk))
            {
                //文件blacklist.bk存在，且小于，则用blacklist.txt将blacklist.bk覆盖
                if (fi_blacklist.Length > fi_blacklistbk.Length)
                {
                    //SetText("blackListBackup: " + "ori = " + origin.Length + " ,bak = " + backup.Length);
                    String sourcePath = path_blacklist;
                    String targetPath = path_blacklistbk;
                    System.IO.File.Copy(sourcePath, targetPath, true);
                }
            }
            else if (File.Exists(path_blacklist) && !File.Exists(path_blacklistbk))
            {
                //文件blacklist.bk不存在，复制blacklist.txt为blacklist.bk
                //SetText("blacklist.txt Exists: " + File.Exists(System.Environment.CurrentDirectory + "\\" + "blacklist.txt").ToString());
                String sourcePath = path_blacklist;
                String targetPath = path_blacklistbk;
                System.IO.File.Copy(sourcePath, targetPath, true);
            }
        }

        //若blacklist.txt变小了，从blacklistbk.txt还原出blacklist.txt
        private void blackListRecover()
        {
            if (File.Exists(path_blacklist) && File.Exists(path_blacklistbk))
            {
                //文件blacklistbk.txt存在，且小于，则用blacklist.txt将blacklistbk.txt覆盖
                if (fi_blacklist.Length < fi_blacklistbk.Length)
                {
                    //SetText("blackListBackup: " + "ori = " + origin.Length + " ,bak = " + backup.Length);
                    String targetPath = path_blacklist;
                    String sourcePath = path_blacklistbk;
                    System.IO.File.Copy(sourcePath, targetPath, true);
                }
            }
            else if (!File.Exists(path_blacklist) && File.Exists(path_blacklistbk))
            {
                //SetText("blacklist.txt Exists: " + File.Exists(System.Environment.CurrentDirectory + "\\" + "blacklist.txt").ToString());
                //文件blacklistbk.txt不存在，复制blacklist.txt为blacklistbk.txt
                String targetPath = path_blacklist;
                String sourcePath = path_blacklistbk;
                System.IO.File.Copy(sourcePath, targetPath, true);
            }
        }

        //将blacklist【txt】读入【list】滤掉重复
        Thread bl2li;
        /*
        bl2li = new Thread(new ThreadStart(blackTxt2List));
        bl2li.Start();
        bl2li.Join();
        */
        private void blackTxt2List()
        {
            System.Threading.Thread.Sleep(100);
            while(true)
            {
                if (!key_bl)
                    System.Threading.Thread.Sleep(100);
                else
                {
                    key_bl = false;
                    lock (olock_bl)
                    {
                        if (File.Exists(path_blacklist))
                        {
                            Stream str_bl = fi_blacklist.Open(FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                            StreamReader sr_bl = new StreamReader(str_bl);
                            while (sr_bl.Peek() >= 0)
                            {
                                string sbl = sr_bl.ReadLine();
                                if (!blacklist.Contains(sbl))
                                    blacklist.Add(sbl);
                            }
                            sr_bl.Close();
                            str_bl.Close();
                            blacknum = blacklist.Count;
                        }
                    }
                    key_bl = true;
                    break;
                }
            }
        }

        Thread exit;
        /*
        exit = new Thread(new ThreadStart(terminate));
        exit.Start();
        exit.Join();
         */
        private void terminate()
        {
            done = true;
            t2.Enabled = false;
            System.Threading.Thread.Sleep(1000);
            this.myclient.Stop_ClientServer();
            System.Threading.Thread.Sleep(1000);
            if (th != null)
                th.Abort();
            while(true)
            {
                if (!key_ini)
                    System.Threading.Thread.Sleep(100);
                else
                {
                    key_ini = false;
                    lock (olock_ini)
                    {
                        Stream str_ini = fi_ini.Open(FileMode.Create, FileAccess.Write, FileShare.ReadWrite);
                        StreamWriter sw_ini = new StreamWriter(str_ini);
                        if (seed.Count > 2)
                        {
                            for (int i = 0; i < 20 && i < seed.Count; i++)
                            {
                                sw_ini.WriteLine(this.getSeed());
                            }
                        }
                        sw_ini.Close();
                        str_ini.Close();
                    }
                    key_ini = true;
                    break;
                }
            }
            if (File.Exists(path_ini) && fi_ini.Length > 10)
            {
                String sourcePath = path_ini;
                String targetPath = path_ibk;
                System.IO.File.Copy(sourcePath, targetPath, true);
            }
            GC.Collect();
            enabledButtons(true);
            SetStat("状态：已停止。");
        }

        //停止搜集
        private void btn_stop_Click(object sender, EventArgs e)
        {
            if (done == true)
            {
                SetStat("状态：已停止。");
                MessageBox.Show("程序已停止！");
                GC.Collect();
            }
            else
            {
                terminate();
            }
        }

        //每隔30分钟清理内存
        public void clearMemory(object source, System.Timers.ElapsedEventArgs e)
        {
            //MainForm.checksame.Clear();
            done = true;
            if (th != null)
                th.Abort();
            th = null;
            this.myclient.Stop_ClientServer();
            GC.Collect();
            SetStat("状态：已停止");

            System.Threading.Thread.Sleep(2000);

            done = false;
            enabledButtons(false);
            th = new Thread(new ThreadStart(run));
            th.Start();
            SetStat("状态：正在搜集...");
        }


        //若需刷新链接，则只重启client,让run()保持运行不动。
        public void checkUrl(object source, System.Timers.ElapsedEventArgs e)
        {
            if (isUrlGood < 2)
            {
                //停止搜集
                this.myclient.Stop_ClientServer();
                System.Threading.Thread.Sleep(2000);
                SetStat("状态：已停止");
                //重新启动搜集
                if (seed.Count < 2)
                    MainForm.surl = "http://blog.sina.com.cn/lm/rank/";
                else
                    MainForm.surl = this.getSeed();
                this.myclient.Start_ClientServer();
                MainForm.addSeed(MainForm.surl);
                SetStat("状态：正在搜集...");
                done = false;
            }
            isUrlGood = 0;
        }

        private void btn_new_Click(object sender, EventArgs e)
        {
            t_surl.Text = this.getSeed();
        }

        public static bool IsFileInUse(string fileName)
        {
            bool inUse = true;
            FileStream fs = null;
            try
            {
                fs = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.None);
                inUse = false;
            }
            catch
            {
            }
            finally
            {
                if (fs != null)
                    fs.Close();
            }
            return inUse;//true表示正在使用,false没有使用
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (Directory.Exists(System.Environment.CurrentDirectory + "\\txts"))
                System.Diagnostics.Process.Start(System.Environment.CurrentDirectory + "\\txts");
            else
                MessageBox.Show("还没有文件生成。" + "\r\n" + System.Environment.CurrentDirectory + "\\txts");
        }
        
    }
}
