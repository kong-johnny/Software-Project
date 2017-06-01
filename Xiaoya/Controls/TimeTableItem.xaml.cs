using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Xiaoya.Assist.Models;
using Xiaoya.Views;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace Xiaoya.Controls
{
    public sealed partial class TimeTableItem : UserControl
    {
        /// <summary>
        /// 课程名称
        /// </summary>
        public string CourseName { get; set; }
        /// <summary>
        /// 课程详情
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// 透明度
        /// </summary>
        public new double Opacity { get; set; }
        /// <summary>
        /// 课程信息
        /// </summary>
        public List<TableCourse> Courses { get; set; }
        /// <summary>
        /// 节次重叠课程
        /// </summary>
        public List<TimeTableItemModel> Items { get; set; }

        public TimeTableItem()
        {
            this.InitializeComponent();
            Opacity = 0.7;
            Courses = new List<TableCourse>();
            Items = new List<TimeTableItemModel>();
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            string content = "========\n该节次课程\n========\n\n";

            foreach (var item in Items)
            {
                content += item.Name + "\n" + item.Description + "\n\n";
            }

            content += "========\n课程详情\n========\n\n";

            foreach (var course in Courses)
            {
                content += course.Name + "\n\n" + 
                    "课程编号：" + course.Code + "\n" +
                    "任课教师：" + course.Teacher + "\n" +
                    "教师编号：" + course.TeacherId + "\n" +
                    "课程学分：" + course.Credit + "\n\n" +
                    "时间地点：\n\n" + course.LocationTime + "\n\n--------\n\n";
            }

            var msgDialog = new CommonDialog(content)
            {
                Title = "课程详情",
            };

            await msgDialog.ShowAsyncQueue();
        }
    }
}
