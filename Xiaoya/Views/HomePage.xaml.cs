using LeanCloud;
using System;
using System.Collections.Generic;
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
using Xiaoya.Helpers;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Xiaoya.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class HomePage : Page
    {
        private App app = (App) Application.Current;
        private bool isLogining = false;

        private Windows.Storage.ApplicationDataContainer localSettings = 
            Windows.Storage.ApplicationData.Current.LocalSettings;

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

            if (app.Assist.IsLogin)
            {
                // logined
                LoginText.Text = "欢迎" + (await app.Assist.GetStudentDetails()).Name + "，点此注销";
            }
            else
            {
                // not logined
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
                    // TODO: Analytics
                });
            });
        }

        private async void LoginButton_Click(object sender, RoutedEventArgs e)
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
            if (await loginDialog.ShowAsync() == ContentDialogResult.Primary)
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

                    await msgDialog.ShowAsync();
                }
                isLogining = false;
            }
        }
    }
}
