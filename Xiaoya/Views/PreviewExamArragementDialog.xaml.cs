using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
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
    public sealed partial class PreviewExamArragementDialog : ContentDialog
    {

        public string n = "1";

        public PreviewExamArragementDialog()
        {
            this.InitializeComponent();
            if(ApiInformation.IsPropertyPresent("Windows.UI.Xaml.Controls.ContentDialog", "DefaultButton"))
            {
                this.DefaultButton = ContentDialogButton.Primary;
            }
        }

        private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            if (NTextBox.Text == "1" || NTextBox.Text == "2" || NTextBox.Text == "3" || NTextBox.Text == "4")
            {
                n = NTextBox.Text;
            }
            else
            {
                NTextBox.Background = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 255, 200, 200));
                args.Cancel = true;
            }
        }

        private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
        }
    }
}
