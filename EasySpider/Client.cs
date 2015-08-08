using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data;
using System.Windows.Forms; 

namespace EasySpider
{
    public class Client
    {
        private ClientServer clientServer = new ClientServer();

        public void Start_ClientServer()
        {
            string constr = string.Empty;
            string starturl = MainForm.surl;
            //MessageBox.Show(starturl);
            //string sql = string.Empty;
            //sql = "select Url from TempSplider";

            DataTable dt = new DataTable();
            //Con2Sql.DataTableBindSQL(sql, dt);

            //for (int i = 0; i < dt.Rows.Count; i++)
            //{
            //    Cyh_UrlStack.Instance.Push(dt.Rows[i][0].ToString());
            //}

            //Cyh_UrlStack.Instance.Push(starturl);
            //MessageBox.Show(Cyh_UrlStack.Instance.Pop());
            //Cyh_UrlStack.Instance.Push(starturl);
            clientServer.Start_AbsThreadManager(starturl);    
        }

        public void Stop_ClientServer()
        {
            clientServer.Stop_AbsThreadManager();   
        }


    }
}
