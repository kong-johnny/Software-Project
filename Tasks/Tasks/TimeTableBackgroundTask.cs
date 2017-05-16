using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Windows.Data.Xml.Dom;
using Windows.UI.Notifications;
using Xiaoya.Helpers;

namespace Tasks
{
    public sealed class TimeTableBackgroundTask : IBackgroundTask
    {
        private Windows.Storage.ApplicationDataContainer localSettings =
            Windows.Storage.ApplicationData.Current.LocalSettings;

        public async void Run(IBackgroundTaskInstance taskInstance)
        {
            BackgroundTaskDeferral deferral = taskInstance.GetDeferral();

            TileHelper.UpdateTile(await TileHelper.GetDefaultTileTimeTable());

            deferral.Complete();
        }

    }
}
