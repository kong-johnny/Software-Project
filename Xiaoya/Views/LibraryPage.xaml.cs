using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Display;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Xiaoya.Assist.Models;
using Xiaoya.Classroom;
using Xiaoya.Helpers;
using Xiaoya.Library.Seat.Models;
using Xiaoya.Library.User;
using Xiaoya.Library.User.Models;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Xiaoya.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class LibraryPage : Page
    {

        private App app = (App)Application.Current;

        private ObservableCollection<BorrowedBook> BorrowedBooks
            = new ObservableCollection<BorrowedBook>();

        private ObservableCollection<ReservationHistory> ReservationHistoryModel
            = new ObservableCollection<ReservationHistory>();

        private static readonly Reservation defaultReservation
            = new Reservation(0, "目前无预约", "", 0, "无", "", "无", "无", false, "", "无");

        private Reservation ReservationModel
            = defaultReservation.Clone();

        private ObservableCollection<Building> BuildingModel
            = new ObservableCollection<Building>();

        private ObservableCollection<Room> RoomModel
            = new ObservableCollection<Room>();

        private Windows.Storage.ApplicationDataContainer localSettings =
            Windows.Storage.ApplicationData.Current.LocalSettings;

        bool isLogining = false;

        public LibraryPage()
        {
            this.InitializeComponent();
            this.Loaded += LibraryPage_Loaded;
        }

        private async void LibraryPage_Loaded(object sender, RoutedEventArgs e)
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
            string username = Convert.ToString(localSettings.Values[AppConstants.LIBRARY_USERNAME_SETTINGS]);
            string password = Convert.ToString(localSettings.Values[AppConstants.LIBRARY_PASSWORD_SETTINGS]);

            if (username != "" && password != "")
            {
                try
                {
                    isLogining = true;

                    LoadingProgressBar.Visibility = Visibility.Visible;
                    LoadingProgressBar2.Visibility = Visibility.Visible;
                    LoadingProgressBar3.Visibility = Visibility.Visible;

                    app.LibraryClient.Username = username;
                    app.LibraryClient.Password = password;
                    var res = await app.LibraryClient.Login();

                    if (res == null)
                    {
                        await LoadBorrowedBooks();
                    }

                    app.SeatClient.Username = username;
                    app.SeatClient.Password = password;
                    var res2 = await app.SeatClient.Login();

                    if (res2.Status == "success")
                    {
                        await LoadReservations();
                        await LoadSeat();
                    }
                }
                finally
                {
                    LoadingProgressBar.Visibility = Visibility.Collapsed;
                    LoadingProgressBar2.Visibility = Visibility.Collapsed;
                    LoadingProgressBar3.Visibility = Visibility.Collapsed;
                    isLogining = false;
                }
            }
        }

        async Task LoadReservations()
        {
            LoadingProgressBar2.Visibility = Visibility.Visible;
            var currentReservation = await app.SeatClient.GetCurrentReservation();
            ReservationModel.Clone(defaultReservation);
            if (currentReservation.Data != null && currentReservation.Data.Count > 0)
            {
                ReservationModel.Clone(currentReservation.Data.First());
            }

            var histories = await app.SeatClient.GetReservationHistory(1, 10);
            ReservationHistoryModel.Clear();
            if (histories.Data != null)
            {
                foreach (var item in histories.Data.Items) ReservationHistoryModel.Add(item);
            }
            LoadingProgressBar2.Visibility = Visibility.Collapsed;
        }

        async Task LoadBorrowedBooks()
        {
            LoadingProgressBar.Visibility = Visibility.Visible;
            BorrowedBooks.Clear();
            var res = await app.LibraryClient.GetBorrowedBooks();
            foreach (var item in res) BorrowedBooks.Add(item);
            LoadingProgressBar.Visibility = Visibility.Collapsed;
        }

        async Task LoadSeat()
        {
            var res = await app.SeatClient.GetBuildings();
            BuildingModel.Clear();
            if (res.Data != null)
            {
                foreach (var item in res.Data.Buildings) BuildingModel.Add(item);
                BuildingComboBox.SelectedIndex = 0;
            }
        }

        private async void Login_Clicked(object sender, RoutedEventArgs e)
        {
            if (isLogining) return;
            // login
            LoginLibraryDialog loginDialog = new LoginLibraryDialog();
            if (await loginDialog.ShowAsyncQueue() == ContentDialogResult.Primary)
            {
                try
                {
                    isLogining = true;

                    LoadingProgressBar.Visibility = Visibility.Visible;
                    LoadingProgressBar2.Visibility = Visibility.Visible;
                    LoadingProgressBar3.Visibility = Visibility.Visible;

                    app.LibraryClient.Username = loginDialog.Username;
                    app.LibraryClient.Password = loginDialog.Password;
                    var res = await app.LibraryClient.Login();

                    if (res == null)
                    {
                        await LoadBorrowedBooks();

                        app.SeatClient.Username = loginDialog.Username;
                        app.SeatClient.Password = loginDialog.Password;
                        var res2 = await app.SeatClient.Login();
                        if (res2.Status == "success")
                        {
                            await LoadReservations();
                            await LoadSeat();
                        }
                        else
                        {
                            var msgDialog = new CommonDialog(res2.Message)
                            {
                                Title = "提示",
                            };

                            await msgDialog.ShowAsyncQueue();
                        }
                    }
                    else
                    {
                        var msgDialog = new CommonDialog(res)
                        {
                            Title = "提示",
                        };

                        await msgDialog.ShowAsyncQueue();
                    }
                }
                finally
                {
                    LoadingProgressBar.Visibility = Visibility.Collapsed;
                    LoadingProgressBar2.Visibility = Visibility.Collapsed;
                    LoadingProgressBar3.Visibility = Visibility.Collapsed;
                    isLogining = false;
                }
            }
        }

        private async void BorrowedBookListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            var item = (BorrowedBook)e.ClickedItem;
            var msgDialog = new CommonDialog("应还日期：\t" + item.ReturnDate +
                            "\n作者：\t\t" + item.Author +
                            "\n位置：\t\t" + item.Building +
                            "\n索书号：\t" + item.Position +
                            "\n罚款：\t\t" + item.Fine)
            {
                Title = item.Title + " " + item.Description,
            };

            await msgDialog.ShowAsyncQueue();
        }

        private async void RenewAll_Clicked(object sender, RoutedEventArgs e)
        {
            LoadingProgressBar.Visibility = Visibility.Visible;
            var res = await app.LibraryClient.RenewAll();

            if (res == null) res = "未知错误";

            var msgDialog = new CommonDialog(res)
            {
                Title = "提示",
            };

            await msgDialog.ShowAsyncQueue();
            LoadingProgressBar.Visibility = Visibility.Collapsed;
        }

        private async void CancelReservation_Click(object sender, RoutedEventArgs e)
        {
            LoadingProgressBar2.Visibility = Visibility.Visible;
            var res = await app.SeatClient.CancelReservation(ReservationModel.Id);
            if (res.Status == "success")
            {
                await LoadReservations();
                await LoadSeat();
            }
            else
            {
                var msgDialog = new CommonDialog(res.Message)
                {
                    Title = "提示",
                };

                await msgDialog.ShowAsyncQueue();
            }
            LoadingProgressBar2.Visibility = Visibility.Collapsed;
        }

        private async void BuildingComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            LoadingProgressBar3.Visibility = Visibility.Visible;
            var item = (Building)BuildingComboBox.SelectedItem;
            if (item == null) return;
            var res = await app.SeatClient.GetRooms(item.Id);
            RoomModel.Clear();
            if (res.Data != null)
            {
                foreach (var room in res.Data) RoomModel.Add(room);
                RoomComboBox.SelectedIndex = 0;
            }
            LoadingProgressBar3.Visibility = Visibility.Collapsed;
        }

        private async void RoomComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            LoadingProgressBar3.Visibility = Visibility.Visible;
            var item = (Room)RoomComboBox.SelectedItem;
            if (item == null) return;

            var time = DateTime.Now.ToString("yyyy-MM-dd");
            if (tomorrowCheckBox.IsChecked.GetValueOrDefault(false))
            {
                time = DateTime.Now.AddDays(1).ToString("yyyy-MM-dd");
            }

            var res = await app.SeatClient.GetSeatLayout(item.RoomId, time);

            SeatLayoutGrid.RowDefinitions.Clear();
            SeatLayoutGrid.ColumnDefinitions.Clear();
            SeatLayoutGrid.Children.Clear();

            if (res.Data != null)
            {
                for (int i = 0; i < res.Data.Rows; ++i)
                    SeatLayoutGrid.RowDefinitions.Add(new RowDefinition()
                    {
                        Height = GridLength.Auto,
                        MinHeight = 20
                    });
                for (int i = 0; i < res.Data.Columns; ++i)
                    SeatLayoutGrid.ColumnDefinitions.Add(new ColumnDefinition()
                    {
                        Width = GridLength.Auto,
                        MinWidth = 20
                    });

                foreach (var seat in res.Data.Layout)
                {
                    var i = Convert.ToInt32(seat.Key);
                    var row = i / 1000;
                    var col = i % 1000;
                    if (seat.Value.Type == "seat")
                    {
                        var button = new Button()
                        {
                            Content = seat.Value.Name,
                        };
                        ToolTipService.SetToolTip(button, seat.Value.Name +
                            "\n状态：" + seat.Value.ShowStatus +
                            "\n电源：" + (seat.Value.Power.GetValueOrDefault(false) ? "有" : "无"));
                        if (seat.Value.Status == "IN_USE")
                        {
                            button.Background = new SolidColorBrush(Color.FromArgb(180, 255, 100, 100));
                        }
                        else if (seat.Value.Status == "FREE")
                        {
                            if (seat.Value.Power.GetValueOrDefault(false))
                                button.Background = new SolidColorBrush(Color.FromArgb(180, 100, 100, 255));
                            else
                                button.Background = new SolidColorBrush(Color.FromArgb(180, 100, 255, 100));
                        }
                        else if (seat.Value.Status == "BOOKED")
                        {
                            button.Background = new SolidColorBrush(Color.FromArgb(180, 155, 120, 100));
                        }
                        else if (seat.Value.Status == "AWAY")
                        {
                            button.Background = new SolidColorBrush(Color.FromArgb(180, 255, 220, 100));
                        }
                        button.DataContext = seat.Value;
                        button.Click += Order_Clicked;
                        SeatLayoutGrid.Children.Add(button);
                        Grid.SetRow(button, row);
                        Grid.SetColumn(button, col);
                    }
                    else if (seat.Value.Type == "word")
                    {
                        var text = new TextBlock()
                        {
                            Text = seat.Value.Name
                        };
                        SeatLayoutGrid.Children.Add(text);
                        Grid.SetRow(text, row);
                        Grid.SetColumn(text, col);
                    }
                }
            }
            LoadingProgressBar3.Visibility = Visibility.Collapsed;
        }

        private async void Order_Clicked(object sender, RoutedEventArgs e)
        {
            var button = (Button)e.OriginalSource;
            if (button == null) return;

            var seat = (SeatLayoutItem)button.DataContext;
            if (seat == null) return;

            var msgDialog = new CommonDialog("状态：" + seat.ShowStatus +
                        "\n电源：" + (seat.Power.GetValueOrDefault(false) ? "有" : "无"))
            {
                Title = "预约：" + seat.Name,
                PrimaryButtonText = "预约",
            };

            if (await msgDialog.ShowAsyncQueue() == ContentDialogResult.Primary)
            {
                var time = DateTime.Now.ToString("yyyy-MM-dd");
                if (tomorrowCheckBox.IsChecked.GetValueOrDefault(false))
                {
                    time = DateTime.Now.AddDays(1).ToString("yyyy-MM-dd");
                }

                // Order
                var chooseTimeDialog = new ChooseSeatTimeDialog(seat.Id.GetValueOrDefault(0), time);
                if (await chooseTimeDialog.ShowAsyncQueue() == ContentDialogResult.Primary)
                {
                    var res = await app.SeatClient.OrderSeat(seat.Id.GetValueOrDefault(0),
                        time,
                        chooseTimeDialog.StartTime.Id,
                        chooseTimeDialog.EndTime.Id
                    );

                    if (res.Status == "success" && res.Data != null)
                    {
                        var dialog = new CommonDialog(res.Data.Location + "\n" +
                                "凭证号：" + res.Data.Receipt + "\n")
                        {
                            Title = "预约成功",
                        };
                        await dialog.ShowAsyncQueue();
                        await LoadReservations();
                        await LoadSeat();
                    }
                    else
                    {
                        var dialog = new CommonDialog(res.Message)
                        {
                            Title = "错误",
                        };
                        await dialog.ShowAsyncQueue();
                    }
                }
            }
        }

        private async void Refresh_Click(object sender, RoutedEventArgs e)
        {
            await LoadReservations();
        }

        private async void CheckIn_Clicked(object sender, RoutedEventArgs e)
        {
            LoadingProgressBar2.Visibility = Visibility.Visible;
            var res = await app.SeatClient.CheckIn();
            var dialog = new CommonDialog(res.Message)
            {
                Title = "提示",
            };
            await dialog.ShowAsyncQueue();
            LoadingProgressBar2.Visibility = Visibility.Collapsed;
        }
        private async void Leave_Clicked(object sender, RoutedEventArgs e)
        {
            LoadingProgressBar2.Visibility = Visibility.Visible;
            var res = await app.SeatClient.Leave();
            var dialog = new CommonDialog(res.Message)
            {
                Title = "提示",
            };
            await dialog.ShowAsyncQueue();
            LoadingProgressBar2.Visibility = Visibility.Collapsed;
        }
        private async void Stop_Clicked(object sender, RoutedEventArgs e)
        {
            LoadingProgressBar2.Visibility = Visibility.Visible;
            var res = await app.SeatClient.Stop();
            var dialog = new CommonDialog(res.Message)
            {
                Title = "提示",
            };
            await dialog.ShowAsyncQueue();
            LoadingProgressBar2.Visibility = Visibility.Collapsed;
        }

        private async void ReservationListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            var item = (ReservationHistory)e.ClickedItem;
            if (item.State == "RESERVE")
            {
                var dialog = new CommonDialog("是否取消预约：" + item.Date + item.Location)
                {
                    Title = "取消预约",
                    PrimaryButtonText = "取消预约",
                };
                if (await dialog.ShowAsyncQueue() == ContentDialogResult.Primary)
                {
                    await app.SeatClient.CancelReservation(Convert.ToInt32(item.Id));
                }
            }
        }

        private void tomorrowCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            RoomComboBox_SelectionChanged(null, null);
        }

    }
}
