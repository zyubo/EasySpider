using System;
using System.Collections.Generic;
using System.Text;

namespace EasySpider
{
    /// <summary>
    /// <para>职责链抽象类</para>
    /// 对于AbsChain采用的是职责链设计模式,
    /// 目的是抽象出网络爬虫处理html的过程,
    /// 因为在spider程序集中并不真正处理如何解析html,
    /// 用户只需重载AbsChain类中的process方法，完成自定义的处理过程
    /// </summary>
    public abstract class Cyh_AbsChain 
    {
        /// <summary>
        /// 责任链中的一个 hander
        /// </summary>
        private Cyh_AbsChain _handler = null;
        internal Cyh_AbsChain Handler
        {
            get
            { return _handler; }
        }

        /// <summary>
        /// 待处理的url
        /// </summary>
        private string _url = string.Empty;
        public string Url
        {
            get { return _url; }
            set { _url = value; }
        }


        /// <summary>
        /// 文本处理过程(Protected abstract)
        /// </summary>
        /// <param name="htmlStream">html文本</param>
        protected abstract void Process(string html);

        /// <summary>
        /// 设置下一个处理节点
        /// </summary>
        /// <param name="handler">下一个处理节点</param>
        public void SetProcessHandler(Cyh_AbsChain handler)
        {
            _handler = handler;
        }

        /// <summary>
        /// Cyh_AbsChain 开始处理
        /// </summary>
        /// <param name="htmlStream">html文本流</param>
        public void Start_AbsChain(string html)
        {
            Process(html); //处理  用户重载方法
            if (Handler != null)
            {
                Handler.Url = Url;
                Handler.Start_AbsChain(html);   
            }
        }
    }
}
