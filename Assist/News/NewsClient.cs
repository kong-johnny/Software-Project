using AngleSharp.Parser.Html;
using CXHttpNS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xiaoya.News
{
    public class NewsClient
    {
        private static HtmlParser m_Parser = new HtmlParser();
        /// <summary>
        /// 国际交流合作处
        /// </summary>
        /// <returns></returns>
        public static async Task<List<News>> GetOIECNews()
        {
            List<News> news = new List<News>();
            try
            {
                var res = await CXHttp.Connect("http://oiec.bnu.edu.cn/zh-hans/jingwaixuexi/")
                    .Get();
                var body = await res.Content();

                var doc = m_Parser.Parse(body);

                var programList = doc.GetElementsByClassName("program_list");
                if (programList.Length > 0)
                {
                    var ul = programList[0];
                    foreach (var div in ul.GetElementsByClassName("title"))
                    {
                        var a = div.GetElementsByTagName("a");
                        if (a.Length > 0)
                        {
                            var item = a[0];
                            var uri = new Uri(new Uri("http://oiec.bnu.edu.cn/zh-hans/jingwaixuexi/"),
                                new Uri(item.GetAttribute("href")));
                            news.Add(new News(item.TextContent.Trim(), "", uri));
                        }
                    }
                }

                if (news.Count == 0)
                {
                    news.Add(new News("请连接校园网", "", new Uri(@"about:blank")));
                }
            }
            catch
            {
                news.Add(new News("获取失败", "", new Uri(@"about:blank")));
            }
            return news;
        }
        /// <summary>
        /// 教务处
        /// </summary>
        /// <returns></returns>
        public static async Task<List<News>> GetJWCNews()
        {
            List<News> news = new List<News>();
            try
            {
                var res = await CXHttp.Connect("http://jwc.bnu.edu.cn/tzgg/index.html")
                    .Get();
                var body = await res.Content("UTF-8");

                var doc = m_Parser.Parse(body);

                var list = doc.GetElementsByClassName("ul_list");
                if (list.Length > 0)
                {
                    var ul = list[0];
                    foreach (var li in ul.GetElementsByTagName("li"))
                    {
                        var a = li.GetElementsByTagName("a");
                        if (a.Length > 0)
                        {
                            var item = a[0];
                            var timeSpan = li.GetElementsByClassName("newtime");
                            if (timeSpan.Length > 0)
                            {
                                var uri = new Uri(new Uri(@"http://jwc.bnu.edu.cn/tzgg/index.html"), item.GetAttribute("href"));
                                news.Add(new News(item.TextContent.Trim(), timeSpan[0].TextContent.Trim(), uri));
                            }
                        }
                    }
                }
            }
            catch
            {
                news.Add(new News("获取失败", "", new Uri(@"about:blank")));
            }

            return news;
        }
        /// <summary>
        /// 图书馆新闻动态
        /// </summary>
        /// <returns></returns>
        public static async Task<List<News>> GetLIBNews()
        {
            List<News> news = new List<News>();
            try
            {
                var res = await CXHttp.Connect("http://www.lib.bnu.edu.cn/-")
                    .Get();
                var body = await res.Content();

                var doc = m_Parser.Parse(body);

                var list = doc.GetElementsByClassName("item-list");
                if (list.Length > 0)
                {
                    foreach (var li in list[0].GetElementsByTagName("li"))
                    {
                        var a = li.GetElementsByTagName("a");
                        if (a.Length > 0)
                        {
                            var span = li.LastChild;
                            if (span != null)
                            {
                                var uri = new Uri(new Uri(@"http://www.lib.bnu.edu.cn/-"), a[0].GetAttribute("href"));
                                news.Add(new News(a[0].TextContent.Trim(), span.TextContent.Trim(), uri));
                            }
                        }
                    }
                }
            }
            catch
            {
                news.Add(new News("获取失败", "", new Uri(@"about:blank")));
            }

            return news;
        }
    }
}
