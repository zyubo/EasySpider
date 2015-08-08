using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EasySpider
{
    internal class Cyh_ObjThread
    {
        private Cyh_WordThread _workThread;
        internal Cyh_WordThread WorkThread
        {
            get { return _workThread; }
            set { _workThread = value; }
        }

        private System.Threading.Thread _thread;
        public System.Threading.Thread Thread
        {
            get { return _thread; }
            set { _thread = value; }
        }    
    }
}
