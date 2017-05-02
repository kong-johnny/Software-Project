using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Xiaoya.Views;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace Xiaoya.Controls
{
    public sealed partial class PageHeader : UserControl
    {
        public PageHeader()
        {
            this.InitializeComponent();

            this.Loaded += (s, a) =>
            {
                MainPage.Current.TogglePaneButtonRectChanged += Current_TogglePaneButtonSizeChanged;
                this.titleBar.Margin = new Thickness(MainPage.Current.TogglePaneButtonRect.Right, 0, 0, 0);
            };
        }

        private void Current_TogglePaneButtonSizeChanged(MainPage sender, Rect e)
        {
            this.titleBar.Margin = new Thickness(e.Right, 0, 0, 0);
        }

        public UIElement HeaderContent
        {
            get { return (UIElement)GetValue(HeaderContentProperty); }
            set { SetValue(HeaderContentProperty, value); }
        }

        // Using a DependencyProperty as the backing store for HeaderContent.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HeaderContentProperty =
            DependencyProperty.Register("HeaderContent", typeof(UIElement), typeof(PageHeader), new PropertyMetadata(DependencyProperty.UnsetValue));
    }
}
