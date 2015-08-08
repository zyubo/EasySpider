using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EasySpider
{
    public class ClientServer:Cyh_AbsThreadManager
    {
        /// <summary> 用户定义的GetChainHeader（） </summary>
        protected override Cyh_AbsChain GetChainHeader()
        {
            ClientChainNode myChainNode = new ClientChainNode();    

            return myChainNode; 
        }
    }
}
