using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Foundation.Metadata;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Content Dialog item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Xiaoya.Views
{
    public sealed partial class ShareCodeDialog : ContentDialog
    {

        public string OnlineShareCode { get; set; }
        public string OfflineShareCode { get; set; }

        public ShareCodeDialog()
        {
            this.InitializeComponent();
            if(ApiInformation.IsPropertyPresent("Windows.UI.Xaml.Controls.ContentDialog", "DefaultButton"))
            {
                this.DefaultButton = ContentDialogButton.Primary;
            }
            if(ApiInformation.IsPropertyPresent("Windows.UI.Xaml.Controls.ContentDialog", "CloseButtonText"))
            {
                this.CloseButtonText = "关闭";
            }
        }

        private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            DataPackage dataPackage = new DataPackage()
            {
                RequestedOperation = DataPackageOperation.Copy
            };
            dataPackage.SetText(OnlineShareCode);
            Clipboard.SetContent(dataPackage);
        }

        private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            DataPackage dataPackage = new DataPackage()
            {
                RequestedOperation = DataPackageOperation.Copy
            };
            dataPackage.SetText(OfflineShareCode);
            Clipboard.SetContent(dataPackage);
        }
    }
}
