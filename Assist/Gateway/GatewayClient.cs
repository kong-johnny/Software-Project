using AngleSharp.Parser.Html;
using Xiaoya.Gateway.Models;
using CXHttpNS;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xiaoya.Helpers;

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


        public async Task<string> Login()
        {
            if (isLoading) return "请等待上次操作完成";
            try
            {
                isLoading = true;
                var res = await CXHttp.Connect("http://gw.bnu.edu.cn:803/srun_portal_pc.php?ac_id=1&")
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
                    .Post();

                var body = await res.Content("UTF-8");
                var info = body.Split(',')[0];

                res = await CXHttp.Connect("http://gw.bnu.edu.cn:803/include/auth_action.php?k=")
                    .Data("action", "get_online_info")
                    .Post();
                body = await res.Content("UTF-8");
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

                string ret = "已用流量：" + (Convert.ToDouble(fields[1]) / 1024 / 1024).ToString("0.##MB") + "\n"
                    + "已用时长：" + h + "时" + m + "分" + s + "秒\n"
                    + "账户余额：" + fields[3] + "元\n";

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
            if (isLoading) return new UserInfo() { Error = "请等待上次操作完成" };
            try
            {
                isLoading = true;
                var res = await CXHttp.Connect("http://172.16.202.201:8069/user/status/" + Username).Get();

                var body = await res.Content();
                var info = JsonConvert.DeserializeObject<UserInfo>(body);
                return info;
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
