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

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Xiaoya.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ClassroomPage : Page
    {

        public ObservableCollection<Building> BuildingModels = new ObservableCollection<Building>();
        public ObservableCollection<Room> RoomModels = new ObservableCollection<Room>();

        public ClassroomPage()
        {
            this.InitializeComponent();
            this.Loaded += ClassroomPage_Loaded;
        }

        private async void ClassroomPage_Loaded(object sender, RoutedEventArgs e)
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

            LoadingProgressBar.Visibility = Visibility.Visible;

            try
            {

                var buildings = await ClassroomClient.GetBuildings();

                BuildingModels.Clear();
                foreach (var item in buildings) BuildingModels.Add(item);
                BuildingComboBox.ItemsSource = BuildingModels;
                RoomListView.ItemsSource = RoomModels;

                LoadingProgressBar.Visibility = Visibility.Collapsed;

                BuildingComboBox.SelectedIndex = 0;
            }
            catch (Exception err)
            {
                LoadingProgressBar.Visibility = Visibility.Collapsed;
                var msgDialog = new CommonDialog
                {
                    Title = "错误",
                    Message = err.Message,
                    CloseButtonText = "确定"
                };
                await msgDialog.ShowAsyncQueue();
            }

            // Prepare for Sharing
            DataTransferManager dataTransferManager = DataTransferManager.GetForCurrentView();
            dataTransferManager.DataRequested += DataTransferManager_DataRequested;
        }

        private async void BuildingComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (BuildingComboBox.SelectedItem != null)
            {
                LoadingProgressBar.Visibility = Visibility.Visible;
                RoomModels.Clear();
                var item = (Building)BuildingComboBox.SelectedItem;
                await Task.Delay(500);
                var rooms = await ClassroomClient.GetRooms(item.Id);
                foreach (var room in rooms) RoomModels.Add(room);
                LoadingProgressBar.Visibility = Visibility.Collapsed;
            }
        }

        private void Share_Clicked(object sender, RoutedEventArgs e)
        {
            try
            {
                DataTransferManager.ShowShareUI();
            }
            catch
            { }
        }

        private void DataTransferManager_DataRequested(DataTransferManager sender, DataRequestedEventArgs args)
        {
            DataRequest request = args.Request;

            string content = "";

            request.Data.SetText(content);
            request.Data.Properties.Title = "空闲自习室 - " + DateTime.Now.ToString();
        }
    }
}
