using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Diagnostics;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Runtime.InteropServices;

namespace EasySpider
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        /// 

        [DllImport("user32.dll", EntryPoint = "FindWindow", CharSet = CharSet.Auto)]
        private extern static IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern int PostMessage(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);

        [STAThread]
        static void Main()
        {
            //Application.ThreadException += new System.Threading.ThreadExceptionEventHandler(Application_ThreadException);
            if (checkEnable())
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                StartKiller();
                MessageBox.Show("过期检查通过！可以继续使用。\r\n（程序将于3秒后自动启动。）", "提示");
                Application.Run(new MainForm());
            }
            else
                MessageBox.Show("软件过期时间：2013年5月11日，已经过期！启动失败。", "提示");
        }

        public const int WM_CLOSE = 0x10;

        private static void StartKiller()
        {
            Timer timer = new Timer();
            timer.Interval = 3000; //3秒启动
            timer.Tick += new EventHandler(Timer_Tick);
            timer.Start();
        }

        private static void Timer_Tick(object sender, EventArgs e)
        {
            KillMessageBox();
            //停止Timer
            ((Timer)sender).Stop();
        }

        private static void KillMessageBox()
        {
            //按照MessageBox的标题，找到MessageBox的窗口
            IntPtr ptr = FindWindow(null, "提示");
            if (ptr != IntPtr.Zero)
            {
                //找到则关闭MessageBox窗口
                PostMessage(ptr, WM_CLOSE, IntPtr.Zero, IntPtr.Zero);
            }
        }
        
        //public static DateTime DataStandardTime()
        //{//返回国际标准时间
        //    //只使用的时间服务器的IP地址，未使用域名
        //    string[,] timeServer = new string[14, 2];
        //    int[] sequence = new int[] { 3, 2, 4, 8, 9, 6, 11, 5, 10, 0, 1, 7, 12 };
        //    timeServer[0, 0] = "time-a.nist.gov";
        //    timeServer[0, 1] = "129.6.15.28";
        //    timeServer[1, 0] = "time-b.nist.gov";
        //    timeServer[1, 1] = "129.6.15.29";
        //    timeServer[2, 0] = "time-a.timefreq.bldrdoc.gov";
        //    timeServer[2, 1] = "132.163.4.101";
        //    timeServer[3, 0] = "time-b.timefreq.bldrdoc.gov";
        //    timeServer[3, 1] = "132.163.4.102";
        //    timeServer[4, 0] = "time-c.timefreq.bldrdoc.gov";
        //    timeServer[4, 1] = "132.163.4.103";
        //    timeServer[5, 0] = "utcnist.colorado.edu";
        //    timeServer[5, 1] = "128.138.140.44";
        //    timeServer[6, 0] = "time.nist.gov";
        //    timeServer[6, 1] = "192.43.244.18";
        //    timeServer[7, 0] = "time-nw.nist.gov";
        //    timeServer[7, 1] = "131.107.1.10";
        //    timeServer[8, 0] = "nist1.symmetricom.com";
        //    timeServer[8, 1] = "69.25.96.13";
        //    timeServer[9, 0] = "nist1-dc.glassey.com";
        //    timeServer[9, 1] = "216.200.93.8";
        //    timeServer[10, 0] = "nist1-ny.glassey.com";
        //    timeServer[10, 1] = "208.184.49.9";
        //    timeServer[11, 0] = "nist1-sj.glassey.com";
        //    timeServer[11, 1] = "207.126.98.204";
        //    timeServer[12, 0] = "nist1.aol-ca.truetime.com";
        //    timeServer[12, 1] = "207.200.81.113";
        //    timeServer[13, 0] = "nist1.aol-va.truetime.com";
        //    timeServer[13, 1] = "64.236.96.53";
        //    int portNum = 13;
        //    string hostName;
        //    byte[] bytes = new byte[1024];
        //    int bytesRead = 0;
        //    System.Net.Sockets.TcpClient client = new System.Net.Sockets.TcpClient();
        //    for (int i = 0; i < 13; i++)
        //    {
        //        hostName = timeServer[sequence[i], 1];
        //        try
        //        {
        //            client.Connect(hostName, portNum);
        //            System.Net.Sockets.NetworkStream ns = client.GetStream();
        //            bytesRead = ns.Read(bytes, 0, bytes.Length);
        //            client.Close();
        //            break;
        //        }
        //        catch (System.Exception)
        //        {
        //        }
        //    }
        //    char[] sp = new char[1];
        //    sp[0] = ' ';
        //    System.DateTime dt = new DateTime();
        //    string str1;
        //    str1 = System.Text.Encoding.ASCII.GetString(bytes, 0, bytesRead);

        //    string[] s;
        //    s = str1.Split(sp);
        //    dt = System.DateTime.Parse(s[1] + " " + s[2]);//得到标准时间
        //    //dt = System.DateTime.Parse(s[1]);//得到标准日期
        //    //dt=dt.AddHours (8);//得到北京时间*/
        //    return dt;

        //}

        public static bool checkEnable()
        {
            //char sp1 = ' ';
            //char sp2 = '/';
            //string[] s;
            //s = DataStandardTime().ToString().Split(sp1)[0].Split(sp2);
            //if (int.Parse(s[0]) * 365 + int.Parse(s[1]) * 30 + int.Parse(s[2]) > 734906) //2013 * 365 + 5 * 30 + 11
            //    return true; //set false to enable expire check
            //else
                return true;
        }
    }
}
