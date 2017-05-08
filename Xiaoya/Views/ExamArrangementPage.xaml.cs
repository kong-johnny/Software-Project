using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using Xiaoya.Assist.Model;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Xiaoya.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ExamArrangementPage : Page
    {

        private App app = (App) Application.Current;

        private List<ExamArrangement> Arrangement;

        private List<ExamRound> Round;
        
        public ExamArrangementPage()
        {
            this.InitializeComponent();
            this.Loaded += ExamArrangementPage_Loaded;
        }

        private async void ExamArrangementPage_Loaded(object sender, RoutedEventArgs e)
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

            if (!app.Assist.IsLogin)
            {
                var msgDialog = new ContentDialog
                {
                    Title = "提示",
                    Content = "请先登录！",
                    CloseButtonText = "确定"
                };

                await msgDialog.ShowAsync();
                if (Frame.CanGoBack)
                {
                    Frame.GoBack();
                }
            }
            else
            {
                LoadingProgressBar.Visibility = Visibility.Visible;

                Round = await app.Assist.GetExamRounds();

                SemesterComboBox.ItemsSource = Round;
                SemesterComboBox.SelectionChanged += SemesterComboBox_SelectionChanged;

                SemesterComboBox.SelectedItem = Round[0];

                LoadingProgressBar.Visibility = Visibility.Collapsed;
            }
        }

        private async void SemesterComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SemesterComboBox.SelectedItem != null)
            {
                LoadingProgressBar.Visibility = Visibility.Visible;

                Arrangement = await app.Assist.GetExamArrangement((ExamRound)SemesterComboBox.SelectedItem);
                ExamArrangementListView.ItemsSource = Arrangement;

                LoadingProgressBar.Visibility = Visibility.Collapsed;
            }
        }

        private async void PreviewAppBarButton_Clicked(object sender, RoutedEventArgs e)
        {
            var dialog = new PreviewExamArragementDialog();
            if (await dialog.ShowAsync() == ContentDialogResult.Primary)
            {
                SemesterComboBox.SelectedItem = null;

                var year = DateTime.Now.Year;
                var month = DateTime.Now.Month;
                var term = 0; // 秋季学期

                if (month < 9)
                {
                    // 春季学期
                    term = 1;
                    year--;
                }

                var round = new ExamRound("", year + "," + term + "," + dialog.n);

                LoadingProgressBar.Visibility = Visibility.Visible;

                Arrangement = await app.Assist.GetExamArrangement(round);
                ExamArrangementListView.ItemsSource = Arrangement;

                LoadingProgressBar.Visibility = Visibility.Collapsed;
            }
        }

        private async void ExamArrangementListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            var item = (ExamArrangement)e.ClickedItem;
            var msgDialog = new ContentDialog
            {
                Title = item.CourseName,
                Content = "学分：" + item.Credit +
                            "\n考核方式：" + item.ExamType +
                            "\n课程类别：" + item.Classification +
                            "\n\n倒计时：" + item.RemainingDays + "天" +
                            "\n时间：" + item.Time +
                            "\n地点：" + item.Location +
                            "\n座号：" + item.Seat,
                CloseButtonText = "确定"
            };

            await msgDialog.ShowAsync();
        }
    }
}
