using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms; 

namespace EasySpider
{
    /// <summary>
    /// UrlStack类非常的简单，
    /// 它采用单构件设计模式，
    /// 整个程序只用到一个UrlStack对象
    /// 并维护了一个数据结构，
    /// 该数据结构用来存储需要爬虫抓取的Url
    /// </summary>
    public class Cyh_UrlStack
    {
        private static Cyh_UrlStack _urlstack = new Cyh_UrlStack();

        /// <summary> stack、用来存放url </summary>
        private Queue<string> _stack = new Queue<string>();

        /// <summary> stack的最大存放数量 </summary>
        private readonly int _maxLength = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["MaxLength"]);

        /// <summary> 构造函数 </summary>
        private Cyh_UrlStack() { }

        /// <summary> UrlStack的实例 </summary>
        public static Cyh_UrlStack Instance
        {
            get { return _urlstack; }
        }

        public void Push(string url)
        {
            if (!MainForm.list.Contains(url))
            {
                MainForm.list.Add(url);
                MainForm.addSeed(url);
                MainForm.isUrlGood++;
                MainForm.urlcount++;
            }
            else //check url occur times
                MainForm.addSame(url);
                //MessageBox.Show(MainForm.list.First());
                //MessageBox.Show("_stack.Count: " + _stack.Count + "  " + "_maxLength:" + _maxLength);
                lock (this)
                {
                    if (!_stack.Contains(url))
                    {
                        if (_stack.Count >= 500)
                        {
                            _stack.Dequeue();   //移除并返回位于 Queue 开始处的对象。
                        }
                        _stack.Enqueue(url);    //将url添加到 Queue 的结尾处。
                        //MessageBox.Show(_stack.First());
                    }
                }
        }

        public string Pop()
        {
            lock (this)
            {
                if (_stack.Count > 0)
                {
                    return _stack.Dequeue();
                }
                else
                {
                    return "";  
                }
            }
        }

        public int Count
        {
            get { return _stack.Count; }
        }

    }
}
