using LeanCloud;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
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
using Xiaoya.Assist.Models;
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

        private Windows.Storage.ApplicationDataContainer localSettings =
            Windows.Storage.ApplicationData.Current.LocalSettings;

        public ObservableCollection<TimeTableModel> Models = new ObservableCollection<TimeTableModel>();

        private MenuFlyout Menu = new MenuFlyout();
        private List<TableSemester> Semesters;

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
            try
            {
                // Init Import
                if (app.Assist.IsLogin)
                {
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
                }
                else
                {
                    MenuFlyoutItem loginFirst = new MenuFlyoutItem()
                    {
                        Text = "请先登录"
                    };
                    loginFirst.Click += LoginFirst_Click;
                    Menu.Items.Add(loginFirst);

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
            catch (Exception err)
            {
                var msgDialog = new CommonDialog(err.Message)
                {
                    Title = "错误",
                };

                await msgDialog.ShowAsyncQueue();
            }
        }

        private void LoginFirst_Click(object sender, RoutedEventArgs e)
        {
            if (Frame.CanGoBack)
            {
                Frame.GoBack();
            }
        }

        private async void FromOfflineShareCode_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new InputDialog()
            {
                Title = "请输入离线分享码",
                Placeholder = "离线分享码"
            };
            if ((await dialog.ShowAsyncQueue()) == ContentDialogResult.Primary)
            {
                try
                {
                    var tableCourses = JsonConvert.DeserializeObject<TableCourses>(dialog.Result);
                    Models.Add(await TimeTableHelper.GenerateTimeTableModel(tableCourses));

                    app.TimeTables.Add(tableCourses);
                    SaveTimeTables();
                }
                catch (Exception err)
                {
                    var msgDialog = new CommonDialog("分享码格式错误：\n" + err.Message)
                    {
                        Title = "错误",
                    };
                    await msgDialog.ShowAsyncQueue();
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
            if ((await dialog.ShowAsyncQueue()) == ContentDialogResult.Primary)
            {
                try
                {
                    var code = dialog.Result;

                    if (code.Contains("："))
                    {
                        code = code.Substring(code.IndexOf("：") + 1).Trim();
                    }

                    var content = AVObject.CreateWithoutData("TimeTable", code);
                    await content.FetchAsync();

                    var tableCourses = JsonConvert.DeserializeObject<TableCourses>(Convert.ToString(content["Content"]));
                    Models.Add(await TimeTableHelper.GenerateTimeTableModel(tableCourses));

                    app.TimeTables.Add(tableCourses);
                    SaveTimeTables();
                }
                catch (Exception err)
                {
                    var msgDialog = new CommonDialog("分享码格式错误：\n" + err.Message)
                    {
                        Title = "错误",
                    };
                    await msgDialog.ShowAsyncQueue();
                }
            }
        }

        private async void SaveTimeTables()
        {
            Windows.Storage.StorageFolder storageFolder =
                Windows.Storage.ApplicationData.Current.LocalFolder;
            Windows.Storage.StorageFile file =
                await storageFolder.CreateFileAsync("timetable.txt", Windows.Storage.CreationCollisionOption.ReplaceExisting);
            await Windows.Storage.FileIO.WriteTextAsync(file, JsonConvert.SerializeObject(app.TimeTables));
        }

        private async void LoadTimeTables()
        {
            try
            {
                TimeTableProgressBar.Visibility = Visibility.Visible;

                if (app.TimeTables == null)
                {
                    Debug.WriteLine("Started: Load Timetable");
                    Windows.Storage.StorageFolder storageFolder =
                        Windows.Storage.ApplicationData.Current.LocalFolder;
                    Windows.Storage.StorageFile file =
                        await storageFolder.GetFileAsync("timetable.txt");
                    string text = await Windows.Storage.FileIO.ReadTextAsync(file);
                    app.TimeTables = JsonConvert.DeserializeObject<List<TableCourses>>(text);
                    Debug.WriteLine("Finished: Load Timetable");
                }

                if (app.TimeTablePage_Models == null)
                {
                    foreach (var table in app.TimeTables)
                    {
                        Models.Add(await TimeTableHelper.GenerateTimeTableModel(table));
                    }
                    app.TimeTablePage_Models = Models.ToList();
                }
                else
                {
                    foreach (var item in app.TimeTablePage_Models) Models.Add(item);
                }
            }
            catch (FileNotFoundException)
            {
                app.TimeTables = new List<TableCourses>();
                SaveTimeTables();
            }
            finally
            {
                TimeTableProgressBar.Visibility = Visibility.Collapsed;
            }
        }

        private async void ImportItem_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var item = (MenuFlyoutItem)e.OriginalSource;
                var semester = Semesters.Find(o => o.Name == item.Text);
                var tableCourses = await app.Assist.GetTableCourses(semester);

                Models.Add(await TimeTableHelper.GenerateTimeTableModel(tableCourses));

                app.TimeTablePage_Models = Models.ToList();
                app.TimeTables.Add(tableCourses);
                app.HomePage_Models = null;
                SaveTimeTables();
            }
            catch (Exception err)
            {
                var msgDialog = new CommonDialog(err.Message)
                {
                    Title = "错误",
                };

                await msgDialog.ShowAsyncQueue();
            }
        }

        private async void Share_Clicked(object sender, RoutedEventArgs e)
        {
            try
            {
                TimeTableProgressBar.Visibility = Visibility.Visible;

                var shareCode = JsonConvert.SerializeObject(app.TimeTables[TablePivot.SelectedIndex]);
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

                TimeTableProgressBar.Visibility = Visibility.Collapsed;

                await dialog.ShowAsyncQueue();
            }
            catch (Exception err)
            {
                TimeTableProgressBar.Visibility = Visibility.Collapsed;
                var msgDialog = new CommonDialog(err.Message)
                {
                    Title = "错误",
                };

                await msgDialog.ShowAsyncQueue();
            }
        }


        private void Delete_Clicked(object sender, RoutedEventArgs e)
        {
            if (Models.Count == 0) return;
            Models.RemoveAt(TablePivot.SelectedIndex);
            app.TimeTables.RemoveAt(TablePivot.SelectedIndex);
            app.TimeTablePage_Models = Models.ToList();
            app.HomePage_Models = null;
            SaveTimeTables();
        }

        private async void DefaultTile_Clicked(object sender, RoutedEventArgs e)
        {
            if (app.TimeTables.Count == 0)
            {
                var dialog = new CommonDialog("请先导入课程表")
                {
                    Title = "提示",
                };

                await dialog.ShowAsyncQueue();
                return;
            }
            var tableCourses = app.TimeTables[TablePivot.SelectedIndex];
            localSettings.Values[AppConstants.TILE_TIMETABLE] = JsonConvert.SerializeObject(tableCourses);
            TileHelper.UpdateTile(await TileHelper.GetDefaultTileTimeTable());

            var msgDialog = new CommonDialog("设置成功！")
            {
                Title = "提示",
            };

            await msgDialog.ShowAsyncQueue();
        }

    }
}
