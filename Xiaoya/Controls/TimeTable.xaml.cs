using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Xiaoya.Assist.Models;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace Xiaoya.Controls
{
    public sealed partial class TimeTable : UserControl
    {
        public static readonly DependencyProperty ItemsProperty =
            DependencyProperty.Register("Items", typeof(List<TimeTableItemModel>), 
                typeof(TimeTable), new PropertyMetadata(new List<TimeTableItemModel>()));

        public List<TimeTableItemModel> Items
        {
            get { return (List<TimeTableItemModel>)GetValue(ItemsProperty); }
            set { SetValue(ItemsProperty, value); Paint(); }
        }

        private static readonly string[] WEEKS = { "周一", "周二", "周三", "周四", "周五", "周六", "周日" };

        private List<TimeTableItem> m_Tiles = new List<TimeTableItem>();

        public TimeTable()
        {
            this.InitializeComponent();
        }

        public void Paint()
        {
            Dictionary<int, TimeTableItem> dict = new Dictionary<int, TimeTableItem>();
            foreach (var tile in m_Tiles)
            {
                TableGrid.Children.Remove(tile);
            }

            foreach (var item in Items)
            {
                TimeTableItem tile;
                int key = item.Day * 10000 + item.Start * 100 + item.End;
                if (dict.ContainsKey(key))
                {
                    tile = dict[key];
                    tile.Opacity = 1;

                    tile.Items.Add(item);
                    if (!tile.Courses.Contains(item.Course))
                    {
                        tile.Courses.Add(item.Course);
                    }
                }
                else
                {
                    tile = new TimeTableItem()
                    {
                        CourseName = item.Name,
                        Description = item.Description
                    };
                    tile.Items.Add(item);
                    tile.Courses.Add(item.Course);

                    dict.Add(key, tile);

                    TableGrid.Children.Add(tile);
                    Grid.SetRow(tile, item.Start - 1);
                    Grid.SetRowSpan(tile, item.End - item.Start + 1);
                    Grid.SetColumn(tile, item.Day - 1);
                    m_Tiles.Add(tile);
                }
            }
        }

    }
}
