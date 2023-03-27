// using AngleSharp.Parser.Html;
using AngleSharp.Html.Parser;
using Xiaoya.Gateway.Models;
using srun_login;
using CXHttpNS;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xiaoya.Helpers;
using System.Net;
using System.IO;
using System.Diagnostics;

using System.ComponentModel;
using Windows.Media.Protection.PlayReady;
using Windows.System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;

namespace Xiaoya.Gateway
{
    public class GatewayClient
    {

        class UserList
        {
            public List<GatewayUser> Items { get; private set; }
            public string Default { get; set; }

            public UserList()
            {
                Items = new List<GatewayUser>();
            }
        }

        private static Windows.Storage.ApplicationDataContainer localSettings =
            Windows.Storage.ApplicationData.Current.LocalSettings;

        public static bool SaveUser(string username, string password)
        {
            UserList list;
            try
            {
                if (localSettings.Values.ContainsKey(AppConstants.GATEWAY_USERS))
                {
                    list =
                        JsonConvert.DeserializeObject<UserList>((string)localSettings.Values[AppConstants.GATEWAY_USERS]);
                }
                else
                {
                    list = new UserList();
                }
            }
            catch
            {
                list = new UserList();
            }
            if (list.Items.Find(o => o.Username == username) != null) return false;
            list.Items.Add(new GatewayUser(username, password));
            localSettings.Values[AppConstants.GATEWAY_USERS] = JsonConvert.SerializeObject(list);

            if (list.Items.Count == 1)
            {
                SetDefaultUser(0);
            }

            return true;
        }

        public static void RemoveUser(int i)
        {
            UserList list;
            try
            {
                if (localSettings.Values.ContainsKey(AppConstants.GATEWAY_USERS))
                {
                    list =
                        JsonConvert.DeserializeObject<UserList>((string)localSettings.Values[AppConstants.GATEWAY_USERS]);
                }
                else
                {
                    list = new UserList();
                }
            }
            catch
            {
                list = new UserList();
            }
            list.Items.RemoveAt(i);
            localSettings.Values[AppConstants.GATEWAY_USERS] = JsonConvert.SerializeObject(list);
        }

        public static bool EditUser(int i, string username, string password)
        {
            UserList list;
            try
            {
                if (localSettings.Values.ContainsKey(AppConstants.GATEWAY_USERS))
                {
                    list =
                        JsonConvert.DeserializeObject<UserList>((string)localSettings.Values[AppConstants.GATEWAY_USERS]);
                }
                else
                {
                    list = new UserList();
                }
            }
            catch
            {
                list = new UserList();
            }
            if (list.Items.Find(o => o.Username == username && o.Password == password) != null) return false;
            list.Items[i].Username = username;
            list.Items[i].Password = password;
            localSettings.Values[AppConstants.GATEWAY_USERS] = JsonConvert.SerializeObject(list);
            return true;
        }

        public static void SetDefaultUser(int i)
        {
            UserList list;
            try
            {
                if (localSettings.Values.ContainsKey(AppConstants.GATEWAY_USERS))
                {
                    list =
                        JsonConvert.DeserializeObject<UserList>((string)localSettings.Values[AppConstants.GATEWAY_USERS]);
                }
                else
                {
                    list = new UserList();
                }
            } 
            catch
            {
                list = new UserList();
            }
            list.Default = list.Items[i].Username;
            localSettings.Values[AppConstants.GATEWAY_USERS] = JsonConvert.SerializeObject(list);
        }

        public static GatewayUser GetDefaultUser()
        {
            try
            {
                if (localSettings.Values.ContainsKey(AppConstants.GATEWAY_USERS))
                {
                    var list = JsonConvert.DeserializeObject<UserList>((string)localSettings.Values[AppConstants.GATEWAY_USERS]);
                    return list.Items.Find(o => o.Username == list.Default);
                }
                else
                {
                    return null;
                }
            }
            catch
            {
                return null;
            }
        }

        public static List<GatewayUser> LoadUsers()
        {
            try
            {
                if (localSettings.Values.ContainsKey(AppConstants.GATEWAY_USERS))
                    return JsonConvert.DeserializeObject<UserList>((string)localSettings.Values[AppConstants.GATEWAY_USERS]).Items;
                else
                    return new List<GatewayUser>();
            }
            catch
            {
                return new List<GatewayUser>();
            }
        }

        public string Username { get; set; }
        public string Password { get; set; }
        private bool isLoading = false;

        private static readonly HttpClient client = new HttpClient();
        private static string baseUrl = "http://172.16.202.201/";
        private static readonly string enc = "srun_bx1";
        private static readonly int n = 200;
        private static readonly int type = 1;
        private static readonly string ac_id = "1";

        private static async Task<string> GetT()
        {
            string t = DateTimeOffset.Now.ToUnixTimeMilliseconds().ToString();
            return t;
        }

        private static async Task<(string challenge, string userIp)> GetChallenge(string username)
        {
            client.DefaultRequestHeaders.Clear();
            client.DefaultRequestHeaders.Add("Accept", "text/javascript, application/javascript, application/ecmascript, application/x-ecmascript, */*; q=0.01");
            client.DefaultRequestHeaders.Add("Accept-Encoding", "gzip, deflate");
            client.DefaultRequestHeaders.Add("Accept-Language", "zh-CN,zh;q=0.8,zh-TW;q=0.7,zh-HK;q=0.5,en-US;q=0.3,en;q=0.2");
            client.DefaultRequestHeaders.Add("Connection", "keep-alive");
            client.DefaultRequestHeaders.Add("Cookie", "lang=zh-CN");
            client.DefaultRequestHeaders.Add("Host", "172.16.202.204");
            client.DefaultRequestHeaders.Add("Referer", "http://172.16.202.204/srun_portal_pc?ac_id=50&theme=bnu");
            client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:92.0) Gecko/20100101 Firefox/92.0");
            client.DefaultRequestHeaders.Add("X-Requested-With", "XMLHttpRequest");

            //string t = DateTimeOffset.Now.ToUnixTimeMilliseconds().ToString();
            string challenge = null;
            string userIp = null;

            string param = $"?callback=jQuery112405953212365516434_{GetT()}"
            + $"&username={username}"
                + $"&_={GetT()}";
            string responseBody = await client.GetStringAsync(baseUrl + "/cgi-bin/get_challenge" + param);
            string jsonString = responseBody.Split(new char[] { '(', ')' })[1];

            foreach (var pair in jsonString.Split(','))
            {
                if (pair.Contains("challenge"))
                {
                    challenge = pair.Split(':')[1].Trim('\"');
                }
                else if (pair.Contains("client_ip"))
                {
                    userIp = pair.Split(':')[1].Trim('\"');
                }
            }

            Console.WriteLine($"challenge = {challenge}");
            Console.WriteLine($"IP = {userIp}");

            return (challenge, userIp);
        }


        private static string CalcInfo(Info data, string key)
        {
            string jsonString = data.ToJsonString();

            string xString = XEncode.Encode(jsonString, key);

            return "{SRBX1}" + Base64Alt.Encode(xString);
        }

        private static string CalcPwd(string data, string key)
        {
            byte[] hash;
            using (HMAC hmac = new HMACMD5(Encoding.Default.GetBytes(key)))
            {
                hash = hmac.ComputeHash(Encoding.Default.GetBytes(data));
            }

            var res = new StringBuilder();
            for (var i = 0; i < hash.Length; i++)
            {
                res.Append(hash[i].ToString("x2"));
            }

            return res.ToString();
        }

        private static string CalcChksum(string data)
        {
            byte[] hash;
            using (SHA1 sha1 = SHA1.Create())
            {

                hash = sha1.ComputeHash(Encoding.Default.GetBytes(data));
            }

            var res = new StringBuilder();
            for (var i = 0; i < hash.Length; i++)
            {
                res.Append(hash[i].ToString("x2"));
            }

            return res.ToString();
        }


public async Task<string> Login()
        {
            if (isLoading) return "请等待上次操作完成";
            try
            {
                isLoading = true;
                string ret = "错误";
                for (var i = 1; i <= 4; ++i)
                {

                    try
                    {
                        string url = "http://172.16.202.201/srun_portal_pc?ac_id=50&theme=bnu";

                        string t = await GetT();
                        var result = await GetChallenge(Username);
                        string challenge = result.challenge;
                        string userIp = result.userIp;
                        var info_param = new Info
                        {
                            username = Username,
                            password = Password,
                            ip = userIp,
                            acid = ac_id,
                            enc_ver = enc
                        };
                        string inf = CalcInfo(info_param, challenge);
                        string hmd5 = CalcPwd(Password, challenge);
                        string chkstr = challenge + Username;
                        chkstr += challenge + hmd5;
                        chkstr += challenge + info_param.acid;
                        chkstr += challenge + userIp;
                        chkstr += challenge + n;
                        chkstr += challenge + type;
                        chkstr += challenge + inf;

                        info_param.password = "{MD5}" + hmd5;
                        string param = $"?callback=jQuery112408670051697866381_{DateTimeOffset.Now.ToUnixTimeMilliseconds()}"
                            + "&action=login"
                            + $"&username={Username}"
                            + $"&password={info_param.password}"
                            + $"&ac_id={ac_id}"
                            + $"&ip={userIp}"
                            + $"&chksum={CalcChksum(chkstr)}"
                            + $"&info={inf.Replace("+", "%2B").Replace("/", "%2F").Replace("=", "%3D")}"
                            + $"&n={n}"
                            + $"&type={type}"
                            + $"&os=Windows+10"
                            + $"&name=Windows"
                            + $"&double_stack=0"
                            + $"&_={DateTimeOffset.Now.ToUnixTimeMilliseconds()}";
                        string callback = "jQuery112408670051697866381_" + DateTimeOffset.Now.ToUnixTimeMilliseconds().ToString();
                        var headers = new Dictionary<string, string>
                        {
                            { "User-Agent", "Mozilla/5.0 (Windows NT 10.0 WOW64 Trident/7.0 rv:11.0) like Gecko" }
                        };
                        foreach (var header in headers)
                        {
                            client.DefaultRequestHeaders.Add(header.Key, header.Value);
                        }
                        string responseBody = await client.GetStringAsync(baseUrl + "/cgi-bin/srun_portal" + param);
                        string jsonString = responseBody.Split(new char[] { '(', ')' })[1];
                        foreach (var pair in jsonString.Split(','))
                        {
                            string key = pair.Split(':')[0];
                            string value = pair.Split(":")[1];
                            if (value.Contains("error"))
                            {
                                if (value != "\"ip_already_online_error\"")
                                {
                                    Debug.WriteLine($"result = {value.Trim('\"')}");
                                    break;
                                }
                            }
                            /*var res = await CXHttp.Connect("http://172.16.202.204:803/srun_portal_pc.php?ac_id=1&")
                                .UseProxy(false)
                                .Data("action", "login")
                                .Data("ac_id", "1")
                                .Data("user_ip", "")
                                .Data("nas_ip", "")
                                .Data("user_mac", "")
                                .Data("url", "")
                                .Data("ajax", "1")
                                .Data("save_me", "1")
                                .Data("username", Username)
                                .Data("password", Password)
                                .Post();*/
                            /*var res = await CXHttp.Connect("http://172.16.202.201/srun_portal_pc?ac_id=1&theme=bnu")
                                .UseProxy(false)
                                .Data("action", "login")
                                .Data("ac_id", "1")
                                .Data("usename", Username)
                                .Data("password", Password)
                                .Data("ip", userIp)
                                .Data("chksum", CalcChksum(chkstr))
                                .Data("info", inf.Replace("+", "%2B").Replace("/", "%2F").Replace("=", "%3D"))
                                .Data("n", n.ToString())
                                .Data("type", type.ToString())
                                .Data("os", "Windows+10")
                                .Data("name", "Windows")
                                .Data("double_stack", "0")
                                .Data("_", t)
                                .Post();*/

                            /*var body = await res.Content("UTF-8");
                            var info = body.Split(',')[0];
                            System.Diagnostics.Debug.WriteLine(info);*/

                            /*res = await CXHttp.Connect("http://172.16.202.204:803/include/auth_action.php?k=")
                                .UseProxy(false)
                                .Data("action", "get_online_info")
                                .Data("ac_id", "1")
                                .Data("usename", Username)
                                .Data("password", Password)
                                .Data("ip", userIp)
                                .Data("chksum", CalcChksum(chkstr))
                                .Data("info", inf.Replace("+", "%2B").Replace("/", "%2F").Replace("=", "%3D"))
                                .Data("n", n.ToString())
                                .Data("type", type.ToString())
                                .Data("os", "Windows+10")
                                .Data("name", "Windows")
                                .Data("double_stack", "0")
                                .Data("_", t)
                                .Post();*/
                            /*body = await res.Content("UTF-8");
                            if (body == "not_online")
                            {
                                return info;
                            }
                            info += "," + body;
                            var fields = info.Split(',');
                            localSettings.Values[AppConstants.GATEWAY_IP] = fields[6];

                            int s = Convert.ToInt32(fields[2]);
                            int m = s / 60;
                            s %= 60;
                            int h = m / 60;
                            m %= 60;


                            ret = "已用流量：" + (Convert.ToDouble(fields[1]) / 1024 / 1024).ToString("0.##MB") + "\n"
                                + "已用时长：" + h + "时" + m + "分" + s + "秒\n"
                                + "账户余额：" + fields[3] + "元\n";*/
                        }
                    }
                    catch
                    {
                        continue;
                    }
                }
                UserInfo user = await GetUserInfo();
                ret = "已用流量：" + user.SumBytes + "GB\n"
                                + "已用时长：" + user.SumSeconds + "\n"
                                + "账户余额：" + user.UserBalance + "元\n";
                return ret;
            }
            catch (Exception e)
            {
                return e.Message;
            }
            finally
            {
                isLoading = false;
            }
        }

        public async Task<UserInfo> GetUserInfo()
        {
            //if (isLoading) return new UserInfo() { Error = "请等待上次操作完成" };
            try
            {
                /*isLoading = true;
                var res = await CXHttp.Connect("http://172.16.202.201:8069/user/status/" + Username).UseProxy(false).Get();

                var body = await res.Content();
                var info = JsonConvert.DeserializeObject<UserInfo>(body);
                return info;*/
                var result = await GetChallenge(Username);
                string challenge = result.challenge;
                string ip = result.userIp;
                string callback = "jQuery112408670051697866381_" + DateTimeOffset.Now.ToUnixTimeMilliseconds().ToString();
                var parameters = new Dictionary<string, string>
                {
                    {"callback", callback},
                    {"_", DateTimeOffset.Now.ToUnixTimeMilliseconds().ToString() }
                };
                var query_params = new FormUrlEncodedContent(parameters);
                var headers = new Dictionary<string, string>
                {
                    { "User-Agent", "Mozilla/5.0 (Windows NT 10.0 WOW64 Trident/7.0 rv:11.0) like Gecko" }
                };
                var handler = new HttpClientHandler();
                handler.ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;
                var tmp_client = new HttpClient(handler);

                foreach (var header in headers)
                {
                    tmp_client.DefaultRequestHeaders.Add(header.Key, header.Value);
                }
                string info_url = "https://172.16.202.201/cgi-bin/rad_user_info";
                //ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;
                //System.Net.ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
                var response = await tmp_client.GetAsync(info_url + "?" + query_params);
                var content = await response.Content.ReadAsStringAsync();
                //Debug.WriteLine(content);
                //Console.WriteLine(content);
                string[] elements = content.Split(',');
                long id = long.Parse(elements[0]);
                long used_bytes = long.Parse(elements[6]);
                double used_GB = Math.Round((double)used_bytes / 1024 / 1024 / 1024, 3);
                string used_sec = elements[7];

                Debug.WriteLine("用户学号: " + id);
                Debug.WriteLine("已使用流量: " + used_GB.ToString("F3"));

                TimeSpan used_time = TimeSpan.FromSeconds(long.Parse(used_sec));
                Debug.WriteLine("已使用时间: " + used_time.ToString(@"hh\:mm\:ss"));

                UserInfo user = new UserInfo
                {
                    Account = id.ToString(),
                    SumBytes = used_GB.ToString("F3"),
                    SumSeconds = used_time.ToString(@"hh\:mm\:ss"),
                    UserBalance = float.Parse(elements[9]).ToString("F2")
                };

                return user;
            }
            catch (Exception e)
            {
                return new UserInfo()
                {
                    Error = e.Message
                };
            }
            finally
            {
                isLoading = false;
            }
        }

        public async Task<string> Logout()
        {
            if (isLoading) return "请等待上次操作完成";
            try
            {
                isLoading = true;
                if (!localSettings.Values.ContainsKey(AppConstants.GATEWAY_IP)) return "尚未登录。若实际已登录，请尝试强制离线。";
                var ip = (string)localSettings.Values[AppConstants.GATEWAY_IP];

                var res = await CXHttp.Connect("http://gw.bnu.edu.cn:803/srun_portal_pc.php")
                    .UseProxy(false)
                    .Data("action", "auto_logout")
                    .Data("ajax", "1")
                    .Data("info", "")
                    .Data("user_ip", ip)
                    .Post();

                return await res.Content("UTF-8");
            }
            catch (Exception e)
            {
                return e.Message;
            }
            finally
            {
                isLoading = false;
            }

        }

        public async Task<string> Force()
        {
            if (isLoading) return "请等待上次操作完成";
            try
            {
                isLoading = true;
                var res = await CXHttp.Connect("http://gw.bnu.edu.cn:803/srun_portal_pc.php")
                    .UseProxy(false)
                    .Data("action", "logout")
                    .Data("ajax", "1")
                    .Data("username", Username)
                    .Data("password", Password)
                    .Post();

                var body = await res.Content("UTF-8");
                return body;
            }
            catch (Exception e)
            {
                return e.Message;
            }
            finally
            {
                isLoading = false;
            }
        }
    }
}
