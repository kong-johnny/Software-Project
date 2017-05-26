using LeanCloud;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Display;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Xiaoya.Assist.Models;
using Xiaoya.Gateway;
using Xiaoya.Helpers;
using Xiaoya.News;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Xiaoya.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class HomePage : Page
    {
        private App app = (App)Application.Current;
        private bool isLogining = false;

        private Windows.Storage.ApplicationDataContainer localSettings =
            Windows.Storage.ApplicationData.Current.LocalSettings;

        public ObservableCollection<OneDayTimeTableModel> TimetableModels
            = new ObservableCollection<OneDayTimeTableModel>();

        public ObservableCollection<News.News> JWCNewsModel
            = new ObservableCollection<News.News>();
        public ObservableCollection<News.News> OIECNewsModel
            = new ObservableCollection<News.News>();
        public ObservableCollection<News.News> LIBNewsModel
            = new ObservableCollection<News.News>();

        public HomePage()
        {
            this.InitializeComponent();
            this.Loaded += HomePage_Loaded;
        }

        private async void HomePage_Loaded(object sender, RoutedEventArgs e)
        {
            double? diagonal = DisplayInformation.GetForCurrentView().DiagonalSizeInInches;

            //move commandbar to page bottom on small screens
            if (diagonal < 7)
            {
                topbar.Visibility = Visibility.Collapsed;
                pageTitleContainer.Visibility = Visibility.Visible;
            }
            else
            {
                topbar.Visibility = Visibility.Visible;
                pageTitleContainer.Visibility = Visibility.Collapsed;
            }

            LoadTimeTables();
            LoadNews();

            if (app.Assist.IsLogin)
            {
                // logined
                LoginText.Text = "欢迎" + (await app.Assist.GetStudentDetails()).Name + "，点此注销";
            }
            else
            {
                // not logined
                try
                {
                    string username = Convert.ToString(localSettings.Values[AppConstants.USERNAME_SETTINGS]);
                    string password = Convert.ToString(localSettings.Values[AppConstants.PASSWORD_SETTINGS]);

                    if (username != "" && password != "")
                    {
                        // auto login
                        isLogining = true;

                        app.Assist.Username = username;
                        app.Assist.Password = password;

                        LoginText.Text = "登录中……";
                        LoginProgressBar.Visibility = Visibility.Visible;

                        var res = await app.Assist.Login();

                        LoginProgressBar.Visibility = Visibility.Collapsed;

                        if (res == null)
                        {
                            LoginText.Text = "欢迎" + (await app.Assist.GetStudentDetails()).Name + "，点此注销";

                            SaveUser();
                        }
                        else
                        {
                            LoginText.Text = "登录以启用所有功能";
                        }

                        isLogining = false;
                    }
                }
                catch
                {
                    LoginText.Text = "登录以启用所有功能";
                }
                finally
                {
                    isLogining = false;
                }
            }
        }

        private async void LoadNews()
        {
            try
            {
                NewsProgressBar.Visibility = Visibility.Visible;

                JWCNewsModel.Clear();
                foreach (var item in await NewsClient.GetJWCNews()) JWCNewsModel.Add(item);
                OIECNewsModel.Clear();
                foreach (var item in await NewsClient.GetOIECNews()) OIECNewsModel.Add(item);
                LIBNewsModel.Clear();
                foreach (var item in await NewsClient.GetLIBNews()) LIBNewsModel.Add(item);
            }
            finally
            {
                NewsProgressBar.Visibility = Visibility.Collapsed;
            }
        }

        private async void SaveTimeTables()
        {
            Windows.Storage.StorageFolder storageFolder =
                Windows.Storage.ApplicationData.Current.LocalFolder;
            Windows.Storage.StorageFile file =
                await storageFolder.CreateFileAsync("timetable.txt", Windows.Storage.CreationCollisionOption.ReplaceExisting);
            await Windows.Storage.FileIO.WriteTextAsync(file, JsonConvert.SerializeObject(app.TimeTables));
        }

        private async void LoadTimeTables()
        {
            try
            {
                TimeTableProgressBar.Visibility = Visibility.Visible;

                if (app.TimeTables == null)
                {
                    Debug.WriteLine("Started: Load Timetable");
                    Windows.Storage.StorageFolder storageFolder =
                        Windows.Storage.ApplicationData.Current.LocalFolder;
                    Windows.Storage.StorageFile file =
                        await storageFolder.GetFileAsync("timetable.txt");
                    string text = await Windows.Storage.FileIO.ReadTextAsync(file);
                    app.TimeTables = JsonConvert.DeserializeObject<List<TableCourses>>(text);
                    Debug.WriteLine("Finished: Load Timetable");
                }

                if (app.HomePage_Models == null)
                {
                    foreach (var table in app.TimeTables)
                    {
                        TimetableModels.Add(await TimeTableHelper.GenerateOneDayTimeTableModel(table));
                    }
                    app.HomePage_Models = TimetableModels.ToList();
                }
                else
                {
                    foreach (var item in app.HomePage_Models) TimetableModels.Add(item);
                }
            }
            catch (FileNotFoundException)
            {
                app.TimeTables = new List<TableCourses>();
                SaveTimeTables();
            }
            finally
            {
                TimeTableProgressBar.Visibility = Visibility.Collapsed;
            }
        }

        private async void SaveUser()
        {
            var studentInfo = await app.Assist.GetStudentDetails();

            var user = new AVUser();

            var username = app.Assist.Username;
            var password = studentInfo.Id + studentInfo.GaokaoId;

            user.Username = username;
            user.Password = password;

            user.Email = user.Username + "@mail.bnu.edu.cn";

            user["RegistrationTime"] = studentInfo.RegistrationTime;
            user["Nationality"] = studentInfo.Nationality;
            user["AdmitSpeciality"] = studentInfo.Speciality;
            user["MiddleSchool"] = studentInfo.MiddleSchool;
            user["ClassName"] = studentInfo.ClassName;
            user["CollegeWill"] = studentInfo.CollegeWill;
            user["SchoolSystem"] = studentInfo.SchoolSystem;
            user["EducationLevel"] = studentInfo.EducationLevel;
            user["Name"] = studentInfo.Name;
            user["Number"] = studentInfo.Number;
            user["College"] = studentInfo.College;
            user["Gender"] = studentInfo.Gender;
            user["Address"] = studentInfo.Address;
            user["Pinyin"] = studentInfo.Pinyin;
            user["mobile"] = studentInfo.Mobile;
            user["RegistrationGrade"] = studentInfo.RegistrationGrade;
            user["Birthday"] = studentInfo.Birthday;

            await user.SignUpAsync().ContinueWith(async t =>
            {
                await AVUser.LogInAsync(username, password).ContinueWith(t2 =>
                {
                });
            });
        }

        private async void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (isLogining)
                    return;

                if (app.Assist.IsLogin)
                {
                    // logout
                    isLogining = true;

                    localSettings.Values[AppConstants.PASSWORD_SETTINGS] = "";
                    app.Assist.Logout();
                    LoginText.Text = "登录以启用所有功能";

                    isLogining = false;
                }

                // login
                LoginDialog loginDialog = new LoginDialog();
                if (await loginDialog.ShowAsyncQueue() == ContentDialogResult.Primary)
                {
                    isLogining = true;
                    app.Assist.Username = loginDialog.Username;
                    app.Assist.Password = loginDialog.Password;

                    LoginText.Text = "登录中……";
                    LoginProgressBar.Visibility = Visibility.Visible;

                    var res = await app.Assist.Login();

                    LoginProgressBar.Visibility = Visibility.Collapsed;

                    if (res == null)
                    {
                        LoginText.Text = "欢迎" + (await app.Assist.GetStudentDetails()).Name + "，点此注销";

                        SaveUser();

                    }
                    else
                    {
                        LoginText.Text = "登录以启用所有功能";
                        var msgDialog = new CommonDialog
                        {
                            Title = "提示",
                            Message = res,
                            CloseButtonText = "确定"
                        };

                        await msgDialog.ShowAsyncQueue();
                    }
                    isLogining = false;
                }
            }
            catch (Exception err)
            {
                LoginText.Text = "登录以启用所有功能";
                var msgDialog = new CommonDialog
                {
                    Title = "错误",
                    Message = err.Message,
                    CloseButtonText = "确定"
                };

                await msgDialog.ShowAsyncQueue();
            }
            finally
            {
                isLogining = false;
            }
        }

        private async void ListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            var item = (News.News)e.ClickedItem;
            if (item != null)
            {
                await Windows.System.Launcher.LaunchUriAsync(item.Url);
            }
        }

        private async void GatewayLogin_Clicked(object sender, RoutedEventArgs e)
        {
            if (GatewayClient.GetDefaultUser() != null)
            {
                GatewayProgressBar.Visibility = Visibility.Visible;
                try
                {
                    app.GatewayClient.Username = GatewayClient.GetDefaultUser().Username;
                    app.GatewayClient.Password = GatewayClient.GetDefaultUser().Password;

                    var res = await app.GatewayClient.Login();
                    ResultText.Text = res;
                }
                finally
                {
                    GatewayProgressBar.Visibility = Visibility.Collapsed;
                }
            }
            else
            {
                ResultText.Text = "请先设置默认网关账号";
            }
        }

        private async void GatewayLogout_Clicked(object sender, RoutedEventArgs e)
        {
            if (GatewayClient.GetDefaultUser() != null)
            {
                GatewayProgressBar.Visibility = Visibility.Visible;
                try
                {
                    app.GatewayClient.Username = GatewayClient.GetDefaultUser().Username;
                    app.GatewayClient.Password = GatewayClient.GetDefaultUser().Password;

                    var res = await app.GatewayClient.Logout();
                    ResultText.Text = res;
                }
                finally
                {
                    GatewayProgressBar.Visibility = Visibility.Collapsed;
                }
            }
            else
            {
                ResultText.Text = "请先设置默认网关账号";
            }
        }

        private async void GatewayForce_Clicked(object sender, RoutedEventArgs e)
        {
            if (GatewayClient.GetDefaultUser() != null)
            {
                GatewayProgressBar.Visibility = Visibility.Visible;
                try
                {
                    app.GatewayClient.Username = GatewayClient.GetDefaultUser().Username;
                    app.GatewayClient.Password = GatewayClient.GetDefaultUser().Password;

                    var res = await app.GatewayClient.Force();
                    ResultText.Text = res;
                }
                finally
                {
                    GatewayProgressBar.Visibility = Visibility.Collapsed;
                }
            }
            else
            {
                ResultText.Text = "请先设置默认网关账号";
            }
        }
    }
}
