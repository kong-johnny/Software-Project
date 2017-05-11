using LeanCloud;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
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
using Xiaoya.Assist.Model;
using Xiaoya.Helpers;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Xiaoya.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class TimetablePage : Page
    {

        private App app = (App)Application.Current;

        public ObservableCollection<TimeTableModel> Models = new ObservableCollection<TimeTableModel>();

        private MenuFlyout Menu = new MenuFlyout();
        private List<TableSemester> Semesters;
        private List<TableCourses> TimeTables = new List<TableCourses>();

        public TimetablePage()
        {
            this.InitializeComponent();
            this.Loaded += TimetablePage_Loaded;
        }

        private async void TimetablePage_Loaded(object sender, RoutedEventArgs e)
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

            LoadTimeTables();

            // Init Import
            Semesters = await app.Assist.GetTableSemesters();
            foreach (var semester in Semesters)
            {
                MenuFlyoutItem item = new MenuFlyoutItem()
                {
                    Text = semester.Name
                };
                item.Click += ImportItem_Click;
                Menu.Items.Add(item);
            }
            Menu.Items.Add(new MenuFlyoutSeparator());
            MenuFlyoutItem fromShareCode = new MenuFlyoutItem()
            {
                Text = "自在线/安卓分享码导入"
            };
            fromShareCode.Click += FromShareCode_Click;
            Menu.Items.Add(fromShareCode);
            MenuFlyoutItem fromOfflineShareCode = new MenuFlyoutItem()
            {
                Text = "自离线分享码导入"
            };
            fromOfflineShareCode.Click += FromOfflineShareCode_Click;
            Menu.Items.Add(fromOfflineShareCode);
        }

        private async void FromOfflineShareCode_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new InputDialog()
            {
                Title = "请输入离线分享码",
                Placeholder = "离线分享码"
            };
            if ((await dialog.ShowAsync()) == ContentDialogResult.Primary)
            {
                try
                {
                    var tableCourses = JsonConvert.DeserializeObject<TableCourses>(dialog.Result);
                    Models.Add(TimeTableHelper.GenerateTimeTableModel(tableCourses));

                    TimeTables.Add(tableCourses);
                    SaveTimeTables();
                }
                catch (Exception err)
                {
                    var msgDialog = new CommonDialog
                    {
                        Title = "错误",
                        Message = "分享码格式错误：\n" + err.Message,
                        CloseButtonText = "确定"
                    };
                    await msgDialog.ShowAsync();
                }
            }
        }
        private async void FromShareCode_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new InputDialog()
            {
                Title = "请输入在线分享码",
                Placeholder = "在线分享码"
            };
            if ((await dialog.ShowAsync()) == ContentDialogResult.Primary)
            {
                try
                {
                    var code = dialog.Result;

                    if (code.Contains("："))
                    {
                        code.Substring(code.IndexOf("：") + 1);
                    }

                    var content = AVObject.CreateWithoutData("TimeTable", code);
                    await content.FetchAsync();

                    var tableCourses = JsonConvert.DeserializeObject<TableCourses>(Convert.ToString(content["Content"]));
                    Models.Add(TimeTableHelper.GenerateTimeTableModel(tableCourses));

                    TimeTables.Add(tableCourses);
                    SaveTimeTables();
                }
                catch (Exception err)
                {
                    var msgDialog = new CommonDialog
                    {
                        Title = "错误",
                        Message = "分享码格式错误：\n" + err.Message,
                        CloseButtonText = "确定"
                    };
                    await msgDialog.ShowAsync();
                }
            }
        }

        private async void SaveTimeTables()
        { 
            Windows.Storage.StorageFolder storageFolder =
                Windows.Storage.ApplicationData.Current.LocalFolder;
            Windows.Storage.StorageFile file =
                await storageFolder.CreateFileAsync("timetable.txt", Windows.Storage.CreationCollisionOption.ReplaceExisting);
            await Windows.Storage.FileIO.WriteTextAsync(file, JsonConvert.SerializeObject(TimeTables));
        }

        private async void LoadTimeTables()
        {
            try
            {
                Windows.Storage.StorageFolder storageFolder =
                    Windows.Storage.ApplicationData.Current.LocalFolder;
                Windows.Storage.StorageFile file =
                    await storageFolder.GetFileAsync("timetable.txt");
                string text = await Windows.Storage.FileIO.ReadTextAsync(file);
                TimeTables = JsonConvert.DeserializeObject<List<TableCourses>>(text);

                foreach (var table in TimeTables)
                {
                    Models.Add(TimeTableHelper.GenerateTimeTableModel(table));
                }
            }
            catch (FileNotFoundException)
            {
                SaveTimeTables();
            }
        }

        private async void ImportItem_Click(object sender, RoutedEventArgs e)
        {
            var item = (MenuFlyoutItem)e.OriginalSource;
            var semester = Semesters.Find(o => o.Name == item.Text);
            var tableCourses = await app.Assist.GetTableCourses(semester);

            Models.Add(TimeTableHelper.GenerateTimeTableModel(tableCourses));

            TimeTables.Add(tableCourses);
            SaveTimeTables();
        }

        private async void Share_Clicked(object sender, RoutedEventArgs e)
        {
            var shareCode = JsonConvert.SerializeObject(TimeTables[TablePivot.SelectedIndex]);
            var onlineCode = "";

            var dialog = new ShareCodeDialog()
            {
                OfflineShareCode = shareCode
            };
            var query = new AVQuery<AVObject>("TimeTable").WhereEqualTo("Content", shareCode);
            var objs = await query.FindAsync();
            if (objs.Count() > 0)
            {
                onlineCode = Convert.ToString(objs.First().ObjectId);
            }
            else
            {
                var code = new AVObject("TimeTable")
                {
                    ["Content"] = shareCode
                };
                await code.SaveAsync();
                onlineCode = code.ObjectId;
            }
            dialog.OnlineShareCode = "欢迎使用北师小鸦，课程表分享码：" + onlineCode;
            await dialog.ShowAsync();
        }


        private void Delete_Clicked(object sender, RoutedEventArgs e)
        {
            Models.RemoveAt(TablePivot.SelectedIndex);
            TimeTables.RemoveAt(TablePivot.SelectedIndex);
            SaveTimeTables();
        }
    }
}
