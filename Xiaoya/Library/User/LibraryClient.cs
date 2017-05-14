using AngleSharp.Parser.Html;
using CXHttpNS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Xiaoya.Library.User.Models;

namespace Xiaoya.Library.User
{
    public class LibraryClient
    {
        public string Username { get; set; }
        public string Password { get; set; }

        private CXSession m_Session = new CXSession();

        private string m_Key = "";
        private HtmlParser m_Parser = new HtmlParser();

        private const string URL_PREFIX = "http://opac.lib.bnu.edu.cn:8080";
        private const string URL_LOGIN_GET = URL_PREFIX + "/F/?func=bor-loan&adm_library=";
        private const string URL_LOGIN_POST = URL_PREFIX + "/pds";
        private const string URL_LOGIN_JUMP = URL_PREFIX + "/F/{0}?func=bor-info";
        private const string URL_BORROWED_BOOKS = URL_PREFIX + "/F/{0}?func=bor-loan&adm_library=BNU51";
        private const string URL_RENEW_ALL = URL_PREFIX + "/F/{0}?func=bor-renew-all&adm_library=BNU51";
        public LibraryClient() { }

        public LibraryClient(string username, string password)
        {
            Username = username;
            Password = password;
        }

        public async Task<string> Login()
        {
            m_Key = "";
            var res = await m_Session.Req
                .Url(URL_LOGIN_GET)
                .ClearCookies()
                .Get();

            var body = await res.Content("UTF-8");

            Match mc = Regex.Match(body, URL_PREFIX + "/F/(.*?)\\?func");
            m_Key = mc.Groups[1].Value;

            res = await m_Session.Req
                .Url(URL_LOGIN_POST)
                .Data("func", "login")
                .Data("calling_system", "bnuhome")
                .Data("term1", "short")
                .Data("selfreg", "")
                .Data("bor_id", Username)
                .Data("bor_verification", Password)
                .Data("url", String.Format(URL_LOGIN_JUMP, m_Key))
                .Post();

            body = await res.Content("UTF-8");

            mc = Regex.Match(body, "href=\"(.*?)\">Click");
            if (mc.Success)
            {
                string urlLogin = mc.Groups[1].Value;
                await m_Session.Req
                    .Url(URL_PREFIX + urlLogin)
                    .Get();
                return null;
            }
            else
            {
                return "用户名密码错误";
            }
        }

        public async Task<List<BorrowedBook>> GetBorrowedBooks()
        {
            var res = await m_Session.Req
                .Url(String.Format(URL_BORROWED_BOOKS, m_Key))
                .Get();

            var books = new List<BorrowedBook>();

            var body = await res.Content("UTF-8");
            var doc = m_Parser.Parse(body);

            var tables = doc.GetElementsByTagName("table");
            foreach (var table in tables)
            {
                var text3 = table.GetElementsByClassName("text3");
                if (text3.Length > 0 &&
                    text3[0].TextContent != null &&
                    text3[0].TextContent.Contains("No."))
                {
                    foreach (var tr in table.GetElementsByTagName("tr"))
                    {
                        if (tr.ClassName == "tr1")
                        {
                            continue;
                        }
                        var td = tr.GetElementsByTagName("td");
                        if (td.Length > 9)
                        {
                            books.Add(new BorrowedBook(
                                title: td[3].TextContent,
                                author: td[2].TextContent,
                                returnDate: td[5].TextContent,
                                fine: td[6].TextContent,
                                building: td[7].TextContent,
                                position: td[8].TextContent,
                                description: td[9].TextContent
                            ));
                        }
                    }
                    break;
                }
            }

            return books;
        }
        
        public async Task<string> RenewAll()
        {
            var res = await m_Session.Req
                .Url(String.Format(URL_RENEW_ALL, m_Key))
                .Get();

            var body = await res.Content("UTF-8");
            var doc = m_Parser.Parse(body);

            var title = doc.GetElementsByClassName("title");
            if (title.Length == 0)
            {
                return "未知错误";
            }
            else
            {
                var info = title[title.Length - 1].TextContent.Trim() + "\n\n";
                var tables = doc.GetElementsByTagName("table");
                if (tables.Length > 4)
                {
                    var table = tables[4];
                    foreach (var tr in table.GetElementsByTagName("tr"))
                    {
                        if (tr.ClassName == "tr1")
                        {
                            continue;
                        }
                        var td = tr.GetElementsByTagName("td");
                        if (td.Length > 8)
                        {
                            info += Convert.ToString(td[1].TextContent) +
                                ": " + Convert.ToString(td[8].TextContent) + "\n";
                        }
                    }
                }
                return info;
            }
        }
    }
}
