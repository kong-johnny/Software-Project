using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Xiaoya.Helpers;

// The Content Dialog item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Xiaoya.Views
{
    public sealed partial class LoginDialog : ContentDialog
    {

        public string Username { get; private set; }
        public string Password { get; private set; }


        private Windows.Storage.ApplicationDataContainer localSettings = 
            Windows.Storage.ApplicationData.Current.LocalSettings;

        public LoginDialog()
        {
            this.InitializeComponent();

            UsernameTextBox.Text = Convert.ToString(localSettings.Values[Constants.USERNAME_SETTINGS]);
            PasswordTextBox.Password = Convert.ToString(localSettings.Values[Constants.PASSWORD_SETTINGS]);
            RememberCheck.IsChecked = true;
        }

        private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            Username = UsernameTextBox.Text.Trim();
            Password = PasswordTextBox.Password;

            if (RememberCheck.IsChecked.HasValue && RememberCheck.IsChecked.Value)
            {
                // Save info
                localSettings.Values[Constants.USERNAME_SETTINGS] = Username;
                localSettings.Values[Constants.PASSWORD_SETTINGS] = Password;
            }
            else
            {
                // Delete saved info
                localSettings.Values[Constants.USERNAME_SETTINGS] = "";
                localSettings.Values[Constants.PASSWORD_SETTINGS] = "";
            }
        }

        private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
        }
    }
}
