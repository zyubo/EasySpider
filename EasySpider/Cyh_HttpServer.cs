using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.IO;
using System.Text.RegularExpressions;

namespace EasySpider
{
    /// <summary>
    /// <para>HTTP服务类</para>
    /// 由于在程序外该类是不可见的，所以声明时用了internal.
    /// </summary>
    internal class Cyh_HttpServer
    {
        public string GetResponse(string url)
        {

            string html = string.Empty;         //文本内容
            string encoding = string.Empty;     //文本格式

            #region MyRegion
            try
            {
                //创建一个hettpReq请求对象，包含要传递的值name
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "get";     //发送方式
                request.ContentType = "text/html";  //Http标头的值
                request.Timeout = 30 * 1000;        //请求超时时间

                byte[] buffer = new byte[1024];

                //使用using的作用，可以在using结束时，回收所有using段内的内存

                //创建一个响应对象，并重请求对象中得到响应对象的事例。
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    using (Stream reader = response.GetResponseStream())    //得到回应过来的流
                    {
                        reader.ReadTimeout = 30 * 1000;

                        #region 处理流
                        //MemoryStream是一个支持存储区为内存的流。
                        using (MemoryStream memory = new MemoryStream())
                        {
                            int index = 1;
                            int sum = 0;

                            //限制的读取的大小不超过100k
                            while (index > 0 && sum < 100 * 1024)
                            {
                                index = reader.Read(buffer, 0, 1024);
                                if (index > 0)
                                {
                                    memory.Write(buffer, 0, index); //将缓存写入memory
                                    sum += index;
                                }
                            }
                            //网页通常使用utf-8或gb2312进行编码                           
                            html = Encoding.GetEncoding("gb2312").GetString(memory.ToArray());  //返回与指定代码页名称关联的编码。 
                            if (string.IsNullOrEmpty(html))
                            {
                                return html;
                            }
                            else
                            {
                                Regex re = new Regex(@"charset=(?<charset>[\s\S]*?)[""|']");
                                Match m = re.Match(html.ToLower());
                                encoding = m.Groups["charset"].ToString();
                            }

                            if (string.IsNullOrEmpty(encoding) || string.Equals(encoding.ToLower(), "gb2312"))
                            {
                                return html;
                            }
                            else
                            {
                                //不是gb2312编码则按charset值的编码进行读取
                                return Encoding.GetEncoding(encoding).GetString(memory.ToArray());
                            }
                        }
                        #endregion
                    }
                }
            }
            #endregion
            catch
            {
                return "";  
            }
        }
    }
}
