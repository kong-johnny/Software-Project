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
                LoginText.Text = "欢迎" + (await app.Assist.GetStudentDetails()).Name + "，点此注销";
            }
        }

        private async void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            if (isLogining)
                return;

            if (app.Assist.IsLogin)
            {
                // logout
                isLogining = true;

                localSettings.Values[Constants.PASSWORD_SETTINGS] = "";
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

                LoginText.Text = "登录以启用所有功能";
                LoginProgressBar.Visibility = Visibility.Collapsed;

                if (res == null)
                {
                    LoginText.Text = "欢迎" + (await app.Assist.GetStudentDetails()).Name + "，点此注销";
                }
                else
                {
                    var msgDialog = new ContentDialog
                    {
                        Title = "提示",
                        Content = res,
                        CloseButtonText = "确定"
                    };

                    await msgDialog.ShowAsync();
                }
                isLogining = false;
            }
        }
    }
}
