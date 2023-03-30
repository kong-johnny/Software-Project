// using AngleSharp.Parser.Html;
using AngleSharp.Html.Parser;
using CXHttpNS;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using Xiaoya.Gateway;

namespace Xiaoya.News
{
    public class NewsClient
    {
        private static HtmlParser m_Parser = new HtmlParser();
        /// <summary>
        /// 国际交流合作处->综合新闻
        /// </summary>
        /// <returns></returns>
        public static async Task<List<News>> GetOIECNews()
        {
            List<News> news = new List<News>();
            try
            {
                /*var res = await CXHttp.Connect("https://oiec.bnu.edu.cn/xsfw/jwxx/")
                    .Get();
                var body = await res.Content();*/
                // 创建一个HttpClient实例
                var client = new HttpClient();
                // 设置基地址
                client.BaseAddress = new Uri("https://www.bnu.edu.cn/");
                // 发送GET请求，并获取响应
                var response = await client.GetAsync("");
                // 检查响应状态码是否成功
                if (response.IsSuccessStatusCode == false)
                {
                    Debug.WriteLine("访问不了啊 " + response.StatusCode);
                }
                // 获取响应内容
                var body = await response.Content.ReadAsStringAsync();
                // 解析HTML文档
                var doc = m_Parser.ParseDocument(body);
                // 获取目标元素
                var programListtmp = doc.GetElementsByClassName("item-info2");
                var programList = programListtmp.ToList();

                if (programList.Count > 0)
                {
                    foreach (var program in programList)
                    {
                        // 获取子元素中的a标签
                        var link = program;
                        // 获取a标签的href属性值
                        var href = link.GetAttribute("href");
                        // 获取a标签下子元素中的h3标签
                        var h3Element = program.GetElementsByTagName("h3");                       // 获取h3标签的文本内容
                        var title = h3Element[0].TextContent;
                        // 输出结果
                        Debug.WriteLine($"链接：{href}");
                        Debug.WriteLine($"标题：{title}");
                        news.Add(new News(title, "", new Uri(href)));
                    }
                    /*var ul = programList[0];
                    foreach (var div in ul.GetElementsByClassName("item-txt01"))
                    {
                        var a = div.GetElementsByTagName("a");
                        if (a.Length > 0)
                        {
                            var item = a[0];
                            var uri = new Uri(new Uri("https://www.bnu.edu.cn/"),
                                new Uri(item.GetAttribute("href")));
                            news.Add(new News(item.TextContent.Trim(), "", uri));
                        }
                    }*/
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
        /// 教务处->头条
        /// </summary>
        /// <returns></returns>
        public static async Task<List<News>> GetJWCNews()
        {
            List<News> news = new List<News>();
            try
            {
                /*var res = await CXHttp.Connect("https://oiec.bnu.edu.cn/xsfw/jwxx/")
                    .Get();
                var body = await res.Content();*/
                // 创建一个HttpClient实例
                var client = new HttpClient();
                // 设置基地址
                client.BaseAddress = new Uri("https://www.bnu.edu.cn/");
                // 发送GET请求，并获取响应
                var response = await client.GetAsync("");
                // 检查响应状态码是否成功
                if (response.IsSuccessStatusCode == false)
                {
                    Debug.WriteLine("访问不了啊 " + response.StatusCode);
                }
                // 获取响应内容
                var body = await response.Content.ReadAsStringAsync();
                // 解析HTML文档
                var doc = m_Parser.ParseDocument(body);
                // 获取目标元素
                var programListtmp = doc.GetElementsByClassName("liBg3");
                var programList = programListtmp.Concat(doc.GetElementsByClassName("liBg4")).ToList();

                if (programList.Count > 0)
                {
                    foreach (var program in programList)
                    {
                        // 获取子元素中的a标签
                        var link = program.GetElementsByTagName("a");
                        // 获取a标签的href属性值
                        var href = link[0].GetAttribute("href");
                        // 获取a标签下子元素中的h3标签
                        var h3Element = program.GetElementsByTagName("h3");                       // 获取h3标签的文本内容
                        var title = h3Element[0].TextContent;
                        // 输出结果
                        Debug.WriteLine($"链接：{href}");
                        Debug.WriteLine($"标题：{title}");
                        news.Add(new News(title, "", new Uri(href)));
                    }
                    /*var ul = programList[0];
                    foreach (var div in ul.GetElementsByClassName("item-txt01"))
                    {
                        var a = div.GetElementsByTagName("a");
                        if (a.Length > 0)
                        {
                            var item = a[0];
                            var uri = new Uri(new Uri("https://www.bnu.edu.cn/"),
                                new Uri(item.GetAttribute("href")));
                            news.Add(new News(item.TextContent.Trim(), "", uri));
                        }
                    }*/
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
        /// 图书馆新闻动态->学术动态
        /// </summary>
        /// <returns></returns>
        public static async Task<List<News>> GetLIBNews()
        {
            List<News> news = new List<News>();
            try
            {
                /*var res = await CXHttp.Connect("https://oiec.bnu.edu.cn/xsfw/jwxx/")
                    .Get();
                var body = await res.Content();*/
                // 创建一个HttpClient实例
                var client = new HttpClient();
                // 设置基地址
                client.BaseAddress = new Uri("https://www.bnu.edu.cn/");
                // 发送GET请求，并获取响应
                var response = await client.GetAsync("");
                // 检查响应状态码是否成功
                if (response.IsSuccessStatusCode == false)
                {
                    Debug.WriteLine("访问不了啊 " + response.StatusCode);
                }
                // 获取响应内容
                var body = await response.Content.ReadAsStringAsync();
                // 解析HTML文档
                var doc = m_Parser.ParseDocument(body);
                // 获取目标元素
                var programListtmp = doc.GetElementsByClassName("d-bg02");
                var programList = programListtmp.ToList();

                if (programList.Count > 0)
                {
                    foreach (var program in programList)
                    {
                        // 获取子元素中的a标签
                        var link = program.GetElementsByTagName("a");
                        // 获取a标签的href属性值
                        var href = link[0].GetAttribute("href");
                        // 获取a标签下子元素中的h3标签
                        var h3Element = program.GetElementsByTagName("h3");                       // 获取h3标签的文本内容
                        var title = h3Element[0].TextContent;
                        // 输出结果
                        Debug.WriteLine($"链接：{href}");
                        Debug.WriteLine($"标题：{title}");
                        news.Add(new News(title, "", new Uri(href)));
                    }
                    /*var ul = programList[0];
                    foreach (var div in ul.GetElementsByClassName("item-txt01"))
                    {
                        var a = div.GetElementsByTagName("a");
                        if (a.Length > 0)
                        {
                            var item = a[0];
                            var uri = new Uri(new Uri("https://www.bnu.edu.cn/"),
                                new Uri(item.GetAttribute("href")));
                            news.Add(new News(item.TextContent.Trim(), "", uri));
                        }
                    }*/
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
    }
}
