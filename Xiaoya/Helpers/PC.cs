using LeanCloud;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Graphics.Display;
using Windows.Security.ExchangeActiveSyncProvisioning;
using Windows.UI.ViewManagement;

namespace Xiaoya.Helpers
{
    public class PC : IAVAnalyticsDevice
    {
        EasClientDeviceInformation deviceInfo = new EasClientDeviceInformation();
        public string access
        {
            get
            {
                return "WIFI";
            }
        }

        public string app_version
        {
            get
            {
                return "1.0";
            }
        }

        public string carrier
        {
            get
            {
                return "NULL";
            }
        }

        public string channel
        {
            get
            {
                return ".NET";
            }
        }

        public string device_id
        {
            get
            {
                return deviceInfo.Id.ToString();
            }
        }

        public string device_model
        {
            get
            {
                return deviceInfo.SystemProductName;
            }
        }

        public string device_brand
        {
            get
            {
                return deviceInfo.SystemManufacturer;
            }
        }

        public string iid
        {
            get
            {
                return null;
            }
        }

        public string language
        {
            get
            {
                return "zh-CN";
            }
        }

        public string mc
        {
            get
            {
                return "00:00:00:00:00:00";
            }
        }

        public string os
        {
            get
            {
                return "Windows";
            }
        }

        public string os_version
        {
            get
            {
                return deviceInfo.SystemFirmwareVersion;
            }
        }

        public string resolution
        {
            get
            {
                var bounds = ApplicationView.GetForCurrentView().VisibleBounds;
                var scaleFactor = DisplayInformation.GetForCurrentView().RawPixelsPerViewPixel;
                var size = new Size(bounds.Width * scaleFactor, bounds.Height * scaleFactor);
                return "" + size.Width + "x" + size.Height;
            }
        }

        public string timezone
        {
            get
            {
                return "";
            }
        }
    }
}
