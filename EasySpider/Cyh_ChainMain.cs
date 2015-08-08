using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EasySpider
{
    /// <summary><para>ChainMain类是对AbsChain类的具体实现</para>
    /// 它的Process方法是个空方法，
    /// 所以你可以把它理解成它就是具体处理职责链上的头节点，
    /// 通过ChainMain类的_handler将处理任务往下传递，
    /// 用户通过调用ChainMain的SetProcessHandler方法设置下一个处理节点，
    /// 这个节点必须由用户继承AbsChain并实现抽象方法Process
    /// </summary>
    internal class Cyh_ChainMain : Cyh_AbsChain
    {
        /// <summary> 需要用户重置的处理函数 </summary>
        protected override void Process(string html)
        {
            
        }
    }
}
