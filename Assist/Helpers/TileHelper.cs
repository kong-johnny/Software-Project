using Microsoft.Toolkit.Uwp.Notifications;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Data.Xml.Dom;
using Windows.UI.Notifications;
using Xiaoya.Assist.Models;

namespace Xiaoya.Helpers
{
    public class TileHelper
    {
        private static Windows.Storage.ApplicationDataContainer localSettings =
            Windows.Storage.ApplicationData.Current.LocalSettings;

        public static async Task<OneDayTimeTableModel> GetDefaultTileTimeTable()
        {
            if (localSettings.Values.ContainsKey(AppConstants.TILE_TIMETABLE))
            {
                var tableCourses =
                    JsonConvert.DeserializeObject<TableCourses>(
                        localSettings.Values[AppConstants.TILE_TIMETABLE].ToString()
                    );

                return await TimeTableHelper.GenerateOneDayTimeTableModel(tableCourses);
            }
            return null;
        }

        private static XmlDocument MakeContent(string title, string desp,
            string title1, string desp1, string title2, string desp2)
        {
            TileContent content = new TileContent()
            {
                Visual = new TileVisual()
                {
                    TileMedium = new TileBinding()
                    {
                        Content = new TileBindingContentAdaptive()
                        {
                            Children =
                            {
                                new AdaptiveText()
                                {
                                    Text = title
                                },

                                new AdaptiveText()
                                {
                                    Text = desp,
                                    HintStyle = AdaptiveTextStyle.CaptionSubtle
                                }
                            }
                        }
                    },

                    TileWide = new TileBinding()
                    {
                        Content = new TileBindingContentAdaptive()
                        {
                            Children =
                            {
                                new AdaptiveText()
                                {
                                    Text = title,
                                },

                                new AdaptiveText()
                                {
                                    Text = desp,
                                    HintStyle = AdaptiveTextStyle.CaptionSubtle
                                }
                            }
                        }
                    },

                    TileLarge = new TileBinding()
                    {
                        Content = new TileBindingContentAdaptive()
                        {
                            Children =
                            {
                                new AdaptiveText()
                                {
                                    Text = title1,
                                    HintStyle = AdaptiveTextStyle.Subtitle
                                },

                                new AdaptiveText()
                                {
                                    Text = desp1,
                                    HintStyle = AdaptiveTextStyle.CaptionSubtle
                                },

                                new AdaptiveText()
                                {
                                    Text = title2,
                                    HintStyle = AdaptiveTextStyle.Subtitle
                                },

                                new AdaptiveText()
                                {
                                    Text = desp2,
                                    HintStyle = AdaptiveTextStyle.CaptionSubtle
                                }
                            }
                        }
                    }
                }
            };
            return content.GetXml();
        }

        static private bool IsCurrentTimeLessThan(int h, int m)
        {
            int currentH = DateTime.Now.Hour, currentM = DateTime.Now.Minute;

            if (currentH < h) return true;
            if (currentH == h && currentM < m) return true;
            return false;
        }

        static private int GetCurrentClassNumber()
        {
            if (IsCurrentTimeLessThan(8, 45))
            {
                return 1;
            }
            else if (IsCurrentTimeLessThan(9, 40))
            {
                return 2;
            }
            else if (IsCurrentTimeLessThan(10, 45))
            {
                return 3;
            }
            else if (IsCurrentTimeLessThan(11, 40))
            {
                return 4;
            }
            else if (IsCurrentTimeLessThan(14, 15))
            {
                return 5;
            }
            else if (IsCurrentTimeLessThan(15, 10))
            {
                return 6;
            }
            else if (IsCurrentTimeLessThan(16, 15))
            {
                return 7;
            }
            else if (IsCurrentTimeLessThan(17, 10))
            {
                return 8;
            }
            else if (IsCurrentTimeLessThan(18, 45))
            {
                return 9;
            }
            else if (IsCurrentTimeLessThan(19, 40))
            {
                return 10;
            }
            else if (IsCurrentTimeLessThan(20, 35))
            {
                return 11;
            }
            else if (IsCurrentTimeLessThan(21, 30))
            {
                return 12;
            }
            return -1;
        }

        public static void UpdateTile(OneDayTimeTableModel courses)
        {
            // Create a tile update manager for the specified syndication feed.
            var updater = TileUpdateManager.CreateTileUpdaterForApplication();
            updater.EnableNotificationQueue(true);
            updater.Clear();

            List<string> titles = new List<string>(), desps = new List<string>();

            foreach (var item in courses.Courses)
            {
                if (item.End < GetCurrentClassNumber()) continue;

                titles.Add(item.Name);
                desps.Add(item.Start + " - " + item.End + "节\n" + item.Description);
            }

            int n = titles.Count();
            int m = n;
            if (n % 2 == 1)
            {
                titles.Add("");
                desps.Add("");
                ++m;
            }
            for (int i = 0; i < Math.Min(n, 4); ++i)
            {
                XmlDocument tileXml =
                    MakeContent(titles[i], desps[i],
                        titles[(i * 2) % m], desps[(i * 2) % m],
                        titles[(i * 2 + 1) % m], desps[(i * 2 + 1) % m]);

                // Create a new tile notification.
                updater.Update(new TileNotification(tileXml));
            }

        }
    }
}
