using LeanCloud;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.Background;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Security.ExchangeActiveSyncProvisioning;
using Windows.System.Profile;
using Windows.UI;
using Windows.UI.StartScreen;
using Windows.UI.ViewManagement;
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
using Xiaoya.Library.Seat;
using Xiaoya.Library.User;
using Xiaoya.Views;

namespace Xiaoya
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    sealed partial class App : Application
    {
        private Windows.Storage.ApplicationDataContainer localSettings =
            Windows.Storage.ApplicationData.Current.LocalSettings;

        public Assistant Assist { get; private set; }
        public LibraryClient LibraryClient { get; private set; }
        public SeatClient SeatClient { get; private set; }
        public GatewayClient GatewayClient { get; private set; }
        public List<TableCourses> TimeTables = null;
        public List<TimeTableModel> TimeTablePage_Models = null;
        public List<OneDayTimeTableModel> HomePage_Models = null;
        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            this.InitializeComponent();
            this.Suspending += OnSuspending;
            Assist = new Assistant();
            LibraryClient = new LibraryClient();
            SeatClient = new SeatClient();
            GatewayClient = new GatewayClient();
            AVClient.Initialize("vXdeiDEvPWNif2dvtCVc7Q1N-9Nh9j0Va", "CVlURpsG9thauLU2xUwnbuFi");
            SavePC();
        }

        /// <summary>
        /// Analytics
        /// </summary>
        private async void SavePC()
        {
            if (localSettings.Values.ContainsKey(AppConstants.ANALYTICS_SAVED)) return;

            var pc = new AVObject("DotNet");

            try
            {
                // get the system family information
                var deviceFamily = AnalyticsInfo.VersionInfo.DeviceFamily;

                // get the system version number
                var deviceFamilyVersion = AnalyticsInfo.VersionInfo.DeviceFamilyVersion;
                var version = ulong.Parse(deviceFamilyVersion);
                var majorVersion = (version & 0xFFFF000000000000L) >> 48;
                var minorVersion = (version & 0x0000FFFF00000000L) >> 32;
                var buildVersion = (version & 0x00000000FFFF0000L) >> 16;
                var revisionVersion = (version & 0x000000000000FFFFL);
                var systemVersion = $"{majorVersion}.{minorVersion}.{buildVersion}.{revisionVersion}";

                // get the device manufacturer, model name, OS details etc.
                var clientDeviceInformation = new EasClientDeviceInformation();

                pc["SystemVer"] = systemVersion;
                pc["SystemManufacturer"] = clientDeviceInformation.SystemManufacturer;
                pc["DeviceModel"] = clientDeviceInformation.SystemProductName;
                pc["OperatingSystem"] = clientDeviceInformation.OperatingSystem;
                pc["SystemHardwareVersion"] = clientDeviceInformation.SystemHardwareVersion;
                pc["systemFirmwareVersion"] = clientDeviceInformation.SystemFirmwareVersion;
                await pc.SaveAsync();
                localSettings.Values[AppConstants.ANALYTICS_SAVED] = true;
            }
            catch
            {
            }
        }

        /// <summary>
        /// Init jump list
        /// </summary>
        private async void InitJumpList()
        {
            JumpList jumpList = await JumpList.LoadCurrentAsync();
            jumpList.SystemGroupKind = JumpListSystemGroupKind.None;
            jumpList.Items.Clear();

            var loginItem = JumpListItem.CreateWithArguments("/Login", "登录网关");
            loginItem.Description = "登录北京师范大学上网认证网关";
            var logoutItem = JumpListItem.CreateWithArguments("/Logout", "注销网关");
            logoutItem.Description = "注销北京师范大学上网认证网关";
            var forceItem = JumpListItem.CreateWithArguments("/ForceLogout", "强制离线网关");
            forceItem.Description = "强制离线北京师范大学上网认证网关";

            jumpList.Items.Add(loginItem);
            jumpList.Items.Add(logoutItem);
            jumpList.Items.Add(forceItem);

            await jumpList.SaveAsync();
        }

        private async void GatewayLogin()
        {
            if (GatewayClient.GetDefaultUser() != null)
            {
                this.GatewayClient.Username = GatewayClient.GetDefaultUser().Username;
                this.GatewayClient.Password = GatewayClient.GetDefaultUser().Password;

                var res = await this.GatewayClient.Login();
                var dialog = new CommonDialog()
                {
                    Title = "提示",
                    Message = res,
                    CloseButtonText = "关闭"
                };
                await dialog.ShowAsync();
            }
        }

        private async void GatewayLogout()
        {
            if (GatewayClient.GetDefaultUser() != null)
            {
                this.GatewayClient.Username = GatewayClient.GetDefaultUser().Username;
                this.GatewayClient.Password = GatewayClient.GetDefaultUser().Password;

                var res = await this.GatewayClient.Logout();
                var dialog = new CommonDialog()
                {
                    Title = "提示",
                    Message = res,
                    CloseButtonText = "关闭"
                };
                await dialog.ShowAsync();
            }
        }

        private async void GatewayForce()
        {
            if (GatewayClient.GetDefaultUser() != null)
            {
                this.GatewayClient.Username = GatewayClient.GetDefaultUser().Username;
                this.GatewayClient.Password = GatewayClient.GetDefaultUser().Password;

                var res = await this.GatewayClient.Force();
                var dialog = new CommonDialog()
                {
                    Title = "提示",
                    Message = res,
                    CloseButtonText = "关闭"
                };
                await dialog.ShowAsync();
            }
        }

        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used such as when the application is launched to open a specific file.
        /// </summary>
        /// <param name="e">Details about the launch request and process.</param>
        protected override void OnLaunched(LaunchActivatedEventArgs e)
        {
            // TODO: Start Menu JumpList Task
            switch (e.Arguments)
            {
                case "/Login":
                    GatewayLogin();
                    break;
                case "/Logout":
                    GatewayLogout();
                    break;
                case "/ForceLogout":
                    GatewayForce();
                    break;
            }

            ApplicationView.GetForCurrentView().SetPreferredMinSize(new Size(320, 200));

            // Darken the window title bar using a color value to match app theme
            ApplicationViewTitleBar titleBar = ApplicationView.GetForCurrentView().TitleBar;
            if (titleBar != null)
            {
                Color titleBarColor = (Color)App.Current.Resources["SystemChromeMediumColor"];
                titleBar.BackgroundColor = titleBarColor;
                titleBar.ButtonBackgroundColor = titleBarColor;
            }

            InitJumpList();

            Frame rootFrame = Window.Current.Content as Frame;

            // Do not repeat app initialization when the Window already has content,
            // just ensure that the window is active
            if (rootFrame == null)
            {
                // Create a Frame to act as the navigation context and navigate to the first page
                rootFrame = new Frame();

                rootFrame.NavigationFailed += OnNavigationFailed;

                if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    //TODO: Load state from previously suspended application
                }

                // Place the frame in the current Window
                Window.Current.Content = rootFrame;
            }

            if (e.PrelaunchActivated == false)
            {
                if (rootFrame.Content == null)
                {
                    // When the navigation stack isn't restored navigate to the first page,
                    // configuring the new page by passing required information as a navigation
                    // parameter
                    rootFrame.Navigate(typeof(MainPage), e.Arguments);
                }
                // Ensure the current window is active
                Window.Current.Activate();
            }

        }

        /// <summary>
        /// Invoked when Navigation to a certain page fails
        /// </summary>
        /// <param name="sender">The Frame which failed navigation</param>
        /// <param name="e">Details about the navigation failure</param>
        void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        }

        /// <summary>
        /// Invoked when application execution is being suspended.  Application state is saved
        /// without knowing whether the application will be terminated or resumed with the contents
        /// of memory still intact.
        /// </summary>
        /// <param name="sender">The source of the suspend request.</param>
        /// <param name="e">Details about the suspend request.</param>
        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            //TODO: Save application state and stop any background activity
            deferral.Complete();
        }

    }
}
