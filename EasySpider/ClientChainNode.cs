using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Data.SqlClient;
using System.Data;

namespace EasySpider
{
    public class ClientChainNode:Cyh_AbsChain
    {
        protected override void Process(string html)
        {
            html += ""; 
            try
            {
                //Regex re = new Regex(@"href=(?<web_url>[\s\S]*?)>|href=""(?<web_url>[\s\S]*?)""|href='(?<web_url>[\s\S]*?)'");
                //Regex re = new Regex("(http:\\/\\/|\\.\\/|\\/)?\\w+(\\.\\w+)*(\\/\\w+(\\.\\w+)?)*(\\/|\\?\\w*=\\w*(&\\w*=\\w*)*)?[\\\"\\\']");
                Regex re = new Regex("href=[\\\"\\\'](http:\\/\\/blog.sina.com.cn\\/u\\/|\\.\\/|\\/)?\\w+(\\.\\w+)*(\\/\\w+(\\.\\w+)?)*(\\/|\\?\\w*=\\w*(&\\w*=\\w*)*)?[\\\"\\\']");
                MatchCollection mc = re.Matches(html);
                foreach (Match m in mc)
                {
                    //string url = m.Groups[1].ToString();
                    string url = m.Value.ToString();
                    //去除头部的'与"
                    if ((url.IndexOf("'") == 0) || (url.IndexOf("\"") == 0))
                    {
                        url = url.Remove(0, 1);
                        if (url.IndexOf("'") != -1)
                        {
                            url = url.Remove(url.IndexOf("'"), 1);
                        }
                        if (url.IndexOf("\"") != -1)
                        {
                            url = url.Remove(url.IndexOf("\""), 1);
                        }
                    }
                    if (url.IndexOf("r") == 1)
                    {
                        url = url.Remove(0, 6);
                    }
                    if (url.IndexOf(" ") != -1)
                    {
                        url = url.Remove(url.IndexOf(" "));
                    }
                    if (url.IndexOf(@"http://") != -1)
                    {
                        url = url.Replace("href=\"", "");
                        url = url.Substring(0, url.Length - 1);
                    }
                    // 大于37 设为空
                    if (url.Length <= 37)
                    {
                        Cyh_UrlStack.Instance.Push(@url);
                    }
                }
                string title = string.Empty;
                re = new Regex(@"<title[\s\S]*?>(?<title>[\s\S]*?)</title>");
                Match temp = re.Match(html.ToLower());
                title = temp.Groups["title"].ToString();
                if (!string.IsNullOrEmpty(title))
                {
                    AddUrl(this.Url, title);
                }
            }
            catch
            {
            }
        }

        private void AddUrl(string url, string title)
        {
            string sql = string.Empty;
            sql = string.Format("insert into SpiderTable(Url,Title) values('{0}','{1}')",url,title);
            //Con2Sql.SQL_Command(sql);   
        }

    }
}
