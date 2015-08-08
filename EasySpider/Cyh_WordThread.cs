using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms; 

namespace EasySpider
{
    /// <summary>
    /// <para>工作线程</para>
    /// <para>WorkThread类是工作线程类，
    /// 每个工作线程类都包括</para>
    /// <para>一个职责链的头节点ChainMain、一个HttpServer类和一个UrlStack，</para>
    /// 其中UrlStack类采用了单构件设计模式，
    /// 所以对于整个应该用程序都是使用一个UrlStack对象。
    /// </summary>
    internal class Cyh_WordThread
    {
        #region 定义头节点ChainMain、HttpServer类和UrlStack
        private Cyh_ChainMain _chainHeader = new Cyh_ChainMain();
        internal Cyh_ChainMain ChainMain
        { get { return _chainHeader; } }

        private Cyh_HttpServer _httpServer = new Cyh_HttpServer();
        internal Cyh_HttpServer HttpServer
        { get { return _httpServer; } }

        public Cyh_UrlStack UrlStack
        { get { return Cyh_UrlStack.Instance; } } 

        private bool _isRun = false;
        public bool IsRun
        { get { return _isRun; } } 
        #endregion

        /// <summary>
        /// <para>工作线程入口函数</para>
        /// Start_WordThread()从UrlStack中取出url，
        /// 并调用Cyh_HttpServer的GetResponse方法取出Url对应网页的HTML代码，
        /// 并将HTML代码传递给职责链的头节点Cyh_ChainMain，
        /// 由它的Start_AbsChain()方法开始处理。
        /// 
        /// 它是先调用自身类的Process方法，
        /// 然后再调用_handler.Start_AbsChain（）方法，
        /// 就这样把处理过程传递下去。
        /// </summary>
        public void Start_WordThread()
        {
            #region Try
            try
            {
                this._isRun = true;
                while (_isRun)
                {
                    string url = this.UrlStack.Pop();
                    //MessageBox.Show(url);
                    //MainForm.t_show.Text += url;
                    if (!string.IsNullOrEmpty(url))
                    {
                        string html = _httpServer.GetResponse(url);
                        if (!string.IsNullOrEmpty(html))
                        {
                            this.ChainMain.Url = url;
                            //处理得到的html
                            this.ChainMain.Start_AbsChain(html);  
                        }
                    }
                }
            } 
            #endregion  
            catch
            {  
   
            } 
        }

        /// <summary>
        /// 停止工作线程
        /// </summary>
        public void Stop_WorkThread()
        {
            this._isRun = false;    
        }

    }
}
