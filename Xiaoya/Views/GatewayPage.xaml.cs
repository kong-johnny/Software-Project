using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;
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
using Xiaoya.Classroom;
using Xiaoya.Classroom.Models;
using Xiaoya.Gateway;
using Xiaoya.Gateway.Models;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Xiaoya.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class GatewayPage : Page
    {

        private App app = (App)Application.Current;

        public ObservableCollection<GatewayUser> GatewayUserModel =
            new ObservableCollection<GatewayUser>();

        public GatewayPage()
        {
            this.InitializeComponent();
            this.Loaded += GatewayPage_Loaded;
        }

        private void GatewayPage_Loaded(object sender, RoutedEventArgs e)
        {
            double? diagonal = DisplayInformation.GetForCurrentView().DiagonalSizeInInches;

            //move commandbar to page bottom on small screens
            if (diagonal < 7)
            {
                topbar.Visibility = Visibility.Collapsed;
                pageTitleContainer.Visibility = Visibility.Visible;
                bottombar.Visibility = Visibility.Visible;
            }
            else
            {
                topbar.Visibility = Visibility.Visible;
                pageTitleContainer.Visibility = Visibility.Collapsed;
                bottombar.Visibility = Visibility.Collapsed;
            }
            LoadUsers();
        }

        private void LoadUsers()
        {
            GatewayUserModel.Clear();
            DefaultUserText.Text = "";

            var list = GatewayClient.LoadUsers();
            foreach (var item in list) GatewayUserModel.Add(item);
            if (list.Count > 0)
                GatewayPivot.SelectedIndex = 0;
            if (GatewayClient.GetDefaultUser() != null)
                DefaultUserText.Text = GatewayClient.GetDefaultUser().Username;

        }

        private async void Add_Clicked(object sender, RoutedEventArgs e)
        {
            var dialog = new GatewayInputDialog();
            if (await dialog.ShowAsyncQueue() == ContentDialogResult.Primary)
            {
                if (!GatewayClient.SaveUser(dialog.Username, dialog.Password))
                {
                    var msgDialog = new CommonDialog()
                    {
                        Title = "提示",
                        Message = "用户已存在",
                        CloseButtonText = "确定"
                    };
                    await msgDialog.ShowAsyncQueue();
                }
            }
            LoadUsers();
        }

        private void Remove_Clicked(object sender, RoutedEventArgs e)
        {
            if (GatewayUserModel.Count == 0) return;

            GatewayClient.RemoveUser(GatewayPivot.SelectedIndex);
            LoadUsers();
        }

        private void Edit_Clicked(object sender, RoutedEventArgs e)
        {
            if (GatewayUserModel.Count == 0) return;

            int i = GatewayPivot.SelectedIndex;
            GatewayClient.EditUser(i, GatewayUserModel[i].Username, GatewayUserModel[i].Password);
            LoadUsers();
        }

        private void Default_Clicked(object sender, RoutedEventArgs e)
        {
            if (GatewayUserModel.Count == 0) return;

            int i = GatewayPivot.SelectedIndex;
            GatewayClient.SetDefaultUser(i);
            LoadUsers();
        }

        private bool SetCurrentUser()
        {
            if (GatewayUserModel.Count == 0) return false;

            int i = GatewayPivot.SelectedIndex;

            app.GatewayClient.Username = GatewayUserModel[i].Username;
            app.GatewayClient.Password = GatewayUserModel[i].Password;
            return true;
        }

        private async void Login_Clicked(object sender, RoutedEventArgs e)
        {
            if (SetCurrentUser())
            {
                LoadingProgressBar.Visibility = Visibility.Visible;
                try
                {
                    var res = await app.GatewayClient.Login();
                    ResultText.Text = res;
                }
                finally
                {
                    LoadingProgressBar.Visibility = Visibility.Collapsed;
                }
            }
        }

        private async void Logout_Clicked(object sender, RoutedEventArgs e)
        {
            if (SetCurrentUser())
            {
                LoadingProgressBar.Visibility = Visibility.Visible;
                try
                {
                    var res = await app.GatewayClient.Logout();
                    ResultText.Text = res;
                }
                finally
                {
                    LoadingProgressBar.Visibility = Visibility.Collapsed;
                }
            }
        }

        private async void Force_Clicked(object sender, RoutedEventArgs e)
        {
            if (SetCurrentUser())
            {
                LoadingProgressBar.Visibility = Visibility.Visible;
                try
                {
                    var res = await app.GatewayClient.Force();
                    ResultText.Text = res;
                }
                finally
                {
                    LoadingProgressBar.Visibility = Visibility.Collapsed;
                }
            }
        }
    }
}
