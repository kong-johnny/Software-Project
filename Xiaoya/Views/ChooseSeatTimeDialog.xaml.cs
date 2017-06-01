using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using Xiaoya.Library.Seat.Models;

// The Content Dialog item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Xiaoya.Views
{
    public sealed partial class ChooseSeatTimeDialog : ContentDialog
    {

        private App app = (App)Application.Current;

        public int SeatId { get; private set; }

        private ObservableCollection<Time> m_StartTimeModel = new ObservableCollection<Time>();
        public ObservableCollection<Time> StartTimeModel { get => m_StartTimeModel; }

        private ObservableCollection<Time> m_EndTimeModel = new ObservableCollection<Time>();
        public ObservableCollection<Time> EndTimeModel { get => m_EndTimeModel; }

        public Time StartTime { get; private set; }
        public Time EndTime { get; private set; }

        public ChooseSeatTimeDialog(int seatId)
        {
            this.InitializeComponent();
            SeatId = seatId;
            LoadStartTimes();
            if(ApiInformation.IsPropertyPresent("Windows.UI.Xaml.Controls.ContentDialog", "DefaultButton"))
            {
                this.DefaultButton = ContentDialogButton.Primary;
            }
        }

        private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            StartTime = (Time)StartTimeComboBox.SelectedItem;
            EndTime = (Time)EndTimeComboBox.SelectedItem;
        }

        private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
        }

        private async void LoadStartTimes()
        {
            if (SeatId == 0) return;
            var startTimes = await app.SeatClient.GetStartTimes(SeatId, DateTime.Now.ToString("yyyy-MM-dd"));
            if (startTimes.Data != null)
            {
                foreach (var time in startTimes.Data.Items)
                {
                    if (time.Id == "now") continue;
                    StartTimeModel.Add(time);
                }
            }
        }

        private async void StartTimeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var endTimes = await app.SeatClient.GetEndTimes(
                SeatId,
                DateTime.Now.ToString("yyyy-MM-dd"),
                ((Time)StartTimeComboBox.SelectedItem).Id
            );
            foreach (var time in endTimes.Data.Items)
            {
                EndTimeModel.Add(time);
            }
        }
    }
}
