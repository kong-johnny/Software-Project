using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xiaoya.Assist.Models
{
    public class TimeTableItemModel
    {
        /// <summary>
        /// 课程名称
        /// </summary>
        public string Name { get; private set; }
        /// <summary>
        /// 课程详情
        /// </summary>
        public string Description { get; private set; }
        /// <summary>
        /// 星期
        /// </summary>
        public int Day { get; private set; }
        /// <summary>
        /// 起始节次
        /// </summary>
        public int Start { get; private set; }
        /// <summary>
        /// 结束节次
        /// </summary>
        public int End { get; private set; }

        public TableCourse Course { get; private set; }

        public TimeTableItemModel(string name, string description, int day,
            int start, int end, TableCourse course)
        {
            Name = name.Trim();
            Description = description.Trim();
            Day = day;
            Start = start;
            End = end;
            Course = course;
        }
    }
}
