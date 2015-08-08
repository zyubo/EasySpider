using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms; 

namespace EasySpider
{
    /// <summary>
    /// <para>AbsThreadManager的主要功能是管理开启WorkThread工作线程，</para>
    /// 与监控线线程的,WorkThread对象与Thread对象一一对应，
    /// 这两个对象都被封在ObjThread对象里
    /// 
    /// 在AbsThreadManagers中用List<ObjThread>来维护一系列的线程对象与WorkThread对象，
    /// 同时在 AbsThreadManagers中增加了一个监控线程，
    /// 用来查看工作线程的工作线程，
    /// 若工作线程死去，由监控线程重新启动。
    /// </summary>
    public abstract class Cyh_AbsThreadManager
    {
        public int _maxThread = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["MaxCount"]);

        /// <summary>  用List<ObjThread>来维护一系列的线程对象与WorkThread对象， </summary>
        internal List<Cyh_ObjThread> list = new List<Cyh_ObjThread>();

        private bool _isRun = false;

        /// <summary> 用来监控线程存活死亡的主线程 </summary>
        private System.Threading.Thread _watchThread = null;

        /// <summary> 当前深度 </summary>
        public int Current { get { return Cyh_UrlStack.Instance.Count; } }

        /// <summary>
        /// 开启服务
        /// </summary>
        /// <param name="url">种子URL</param>
        public void Start_AbsThreadManager(string url)
        {
            Cyh_UrlStack.Instance.Push(url);
            //MessageBox.Show(Cyh_UrlStack.Instance.Pop());
            _isRun = true;
            //初始化线程list
            for (int i = 0; i < _maxThread && list.Count <= _maxThread; i++)
            {
                this.AddObjThread();
                //MessageBox.Show("kkk");
            }
            _watchThread = new System.Threading.Thread(Watch);
            _watchThread.Start();   
        }

        /// <summary> 停止服务 </summary>
        public void Stop_AbsThreadManager()
        {
            //MessageBox.Show("before _watchThread.Join()");///////
            _isRun = false;
            if(_watchThread!=null)
                _watchThread.Join();       //阻塞调用线程，直到线程终止为止。
            _watchThread.Abort();
            if (list.Count > 0)
            {
                //MessageBox.Show("_watchThread.Join() done, in if list.count>0");
                for (int i = 1; i < list.Count && list.ElementAt(i) != null; i++)
                {
                    list.ElementAt(i).WorkThread.Stop_WorkThread();
                    list.ElementAt(i).Thread.Abort();
                    list.ElementAt(i).Thread.Join();
                    //MessageBox.Show("after list.ElementAt(i).Thread.Join(), list.count="+list.Count+" i="+i);///////
                }

                //foreach (Cyh_ObjThread obj in list)
                //{
                //    obj.WorkThread.Stop_WorkThread();
                //    obj.Thread.Abort();
                //    obj.Thread.Join();
                //}
                //list.RemoveRange(0, list.Count);
                if(list != null && list.Count > 0)
                list.Clear();
                GC.Collect();
            }

        }

        /// <summary> 增加一个线程 </summary>
        private void AddObjThread()
        {
            Cyh_ObjThread thread = new Cyh_ObjThread(); 
            //初始化一个新的Thread
            thread.WorkThread = new Cyh_WordThread();
            //设置该线程用于处理职责链中的下一个节点
            thread.WorkThread.ChainMain.SetProcessHandler(GetChainHeader());
            thread.Thread = new System.Threading.Thread(thread.WorkThread.Start_WordThread);

            list.Add(thread);   //线程list中加入新的thread
            //MessageBox.Show("552");
            thread.Thread.Start();  //开启该线程
        }

        /// <summary>
        /// <para>设置职责链头节点，该方法由用户设定</para>
        /// 返回一个继承了Cyh_AbsChain类的对象，
        /// 这个对象将会被设置到 Cyh_ChainMain的_handler中
        /// </summary>
        /// <returns>返回用户定义的Chain</returns>
        protected abstract Cyh_AbsChain GetChainHeader();

        /// <summary>
        /// 监测存活的或正在运行的线程，
        /// 将运行结束或死亡的进程去除，
        /// 并新增线程 
        /// </summary>
        internal void Watch()
        {
            List<Cyh_ObjThread> newList = new List<Cyh_ObjThread>();
            while (this._isRun)
            {
                try
                {   //检测存活的线程并保存下来，
                    foreach (Cyh_ObjThread temp in this.list)
                    {
                        if (temp.WorkThread.IsRun && temp.Thread.IsAlive)
                        {
                            newList.Add(temp);
                        }
                    }
                    //更新list中的线程
                    this.list.RemoveRange(0, list.Count);
                    list.AddRange(newList);

                    int newCount = _maxThread - this.list.Count;

                    //加入其它新的线程，使list中的线程数达到_maxThread
                    for (int i = 0; i < newCount; i++)
                    {
                        this.AddObjThread();
                    }
                    newList.RemoveRange(0, newList.Count);
                    //System.Threading.Thread.Sleep(5 * 1000);
                }
                catch
                { }
                finally
                { 
                    
                }
            }
        }
    }
}
