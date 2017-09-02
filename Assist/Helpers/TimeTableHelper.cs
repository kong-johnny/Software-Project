using CXHttpNS;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xiaoya.Assist.Models;

namespace Xiaoya.Helpers
{
    public class TimeTableHelper
    {

        private const string USER_AGENT = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/56.0.2924.87 Safari/537.36";

        private static readonly DayOfWeek[] WEEKS =
        {
            DayOfWeek.Monday,
            DayOfWeek.Tuesday,
            DayOfWeek.Wednesday,
            DayOfWeek.Thursday,
            DayOfWeek.Friday,
            DayOfWeek.Saturday,
            DayOfWeek.Sunday
        };

        private static int m_Week = -1;
        private static DateTime m_Today = DateTime.Now;

        private class Semester
        {
            public string Xn { get; set; }
            public string XqM { get; set; }
        }

        private static Windows.Storage.ApplicationDataContainer localSettings =
            Windows.Storage.ApplicationData.Current.LocalSettings;

        /// <summary>
        /// Get week number of today for specific semester
        /// </summary>
        /// <param name="year">If <c>null</c> is provided, current semester will be used</param>
        /// <param name="semester"></param>
        /// <returns></returns>
        public static async Task<int> GetCurrentWeek(string year = null, string semester = null)
        {
            try
            {
                if (m_Week != -1 && m_Today.Date != DateTime.Now.Date)
                {
                    return m_Week;
                }

                m_Today = DateTime.Now;

                if (year == null)
                {
                    var res = await CXHttp.Connect("http://zyfw.prsc.bnu.edu.cn/jw/common/showYearTerm.action")
                        .Header("User-Agent", USER_AGENT)
                        .Get();
                    var body = await res.Content();

                    Semester s = JsonConvert.DeserializeObject<Semester>(body);

                    year = s.Xn;
                    semester = s.XqM;
                }

                var res2 = await CXHttp.Connect("http://zyfw.prsc.bnu.edu.cn/public/getTeachingWeekByDate.action")
                    .Header("User-Agent", USER_AGENT)
                    .Data("xn", year)
                    .Data("xq_m", semester)
                    .Data("hidOption", "getWeek")
                    .Data("hdrq", DateTime.Now.ToString("yyyy-MM-dd"))
                    .Post();
                var body2 = await res2.Content();

                string[] weeks = body2.Split('@');
                m_Week = Convert.ToInt32(weeks[0]);
                localSettings.Values[AppConstants.TIMETABLE_WEEK] = m_Week;
                localSettings.Values[AppConstants.TIMETABLE_DATE] = DateTime.Now.ToBinary();
                return m_Week;
            }
            catch
            {
                if (localSettings.Values.ContainsKey(AppConstants.TIMETABLE_WEEK) &&
                    localSettings.Values.ContainsKey(AppConstants.TIMETABLE_DATE))
                {
                    m_Week = (int)localSettings.Values[AppConstants.TIMETABLE_WEEK];
                    var then = DateTime.FromBinary((long)localSettings.Values[AppConstants.TIMETABLE_DATE]);
                    int diffDay = (DateTime.Now - then).Days;
                    for (int i = 0; i < 7; ++i)
                    {
                        if (then.DayOfWeek == WEEKS[i])
                        {
                            diffDay -= i;
                        }
                    }
                    int diffWeek = diffDay / 7;
                    m_Week += diffWeek;
                    localSettings.Values[AppConstants.TIMETABLE_WEEK] = m_Week;
                    localSettings.Values[AppConstants.TIMETABLE_DATE] = DateTime.Now.ToBinary();
                    return m_Week;
                }
                return 1;
            }
        }

        /// <summary>
        /// Parse a table course
        /// </summary>
        /// <param name="course"></param>
        /// <param name="weekNumber"></param>
        /// <param name="weekCount"></param>
        /// <returns></returns>
        private static List<TimeTableItemModel> ParseTableCourse(TableCourse course, int weekNumber, ref int weekCount)
        {
            List<TimeTableItemModel> models = new List<TimeTableItemModel>();

            // 免听，无课程表项
            if (course.IsFreeToListen) return models;

            // 待分析文本：s
            string s = course.LocationTime;

            int start = 0;                  // 起始检索位置
            int index = s.IndexOf("周");    // 检索索引

            while (index != -1)
            {
                // 周数范围
                string weekPart = s.Substring(start, index - start).Trim();
                // 每一个周数范围
                string[] weekParts = weekPart.Split(',');
                // 指定周是否在其间
                bool isIn = false;

                foreach (var part in weekParts)
                {
                    // 查找范围符号
                    int rangeSignIndex = part.IndexOf("-");
                    if (rangeSignIndex == -1)
                    {
                        // 如果没有找到，表示这是一个独立的周数
                        int week = Convert.ToInt32(part);
                        if (week > weekCount)
                        {
                            // 更新最大周数
                            weekCount = week;
                        }
                        if (week == weekNumber)
                        {
                            // 指定周就是该课程周数
                            isIn = true;
                        }
                    }
                    else
                    {
                        // 找到范围
                        int startWeek
                            = Convert.ToInt32(part.Substring(0, rangeSignIndex).Trim());
                        int endWeek
                            = Convert.ToInt32(part.Substring(rangeSignIndex + 1).Trim());
                        if (endWeek > weekCount)
                        {
                            // 更新最大周数
                            weekCount = endWeek;
                        }

                        if (weekNumber <= endWeek && weekNumber >= startWeek)
                        {
                            // 指定周位于范围内
                            isIn = true;
                        }
                    }
                }

                // 如果指定周有这个课
                if (isIn)
                {
                    // 星期，起始节次，结束节次
                    int day = 0, startN = 0, endN = 0;

                    // 更新起始检索位置
                    start = index + 1;

                    // 下一个字符是 (，即标记单双周
                    if (s.Substring(start, 1) == "(")
                    {
                        // 那指定周也许并没有这个课，还得判断一下单双周
                        isIn = false;
                        // 奇偶
                        string parity = s.Substring(start + 1, 1);

                        if ((parity == "单" && weekNumber % 2 == 1) ||
                            (parity == "双" && weekNumber % 2 == 0))
                        {
                            isIn = true;
                            // 更新起始检索位置
                            start = s.IndexOf(")", start) + 1;
                        }
                    }

                    // 如果仍然是有课的
                    if (isIn)
                    {
                        // 查找 [
                        index = s.IndexOf("[", start);
                        // 在start到[之间为星期
                        string dayPart = s.Substring(start, index - start).Trim();
                        // 获取星期
                        switch (dayPart)
                        {
                            case "一":
                                day = 1;
                                break;
                            case "二":
                                day = 2;
                                break;
                            case "三":
                                day = 3;
                                break;
                            case "四":
                                day = 4;
                                break;
                            case "五":
                                day = 5;
                                break;
                            case "六":
                                day = 6;
                                break;
                            case "日":
                                day = 7;
                                break;
                        }

                        // 更新起始检索位置
                        start = index + 1;
                        // 查找 ]
                        index = s.IndexOf("]", start);
                        // start到]之间是节次范围
                        string nPart = s.Substring(start, index - start);

                        // 判断是单节还是范围
                        if (nPart.Contains("-"))
                        {
                            // 范围
                            string[] nParts = nPart.Split('-');
                            startN = Convert.ToInt32(nParts[0]);
                            endN = Convert.ToInt32(nParts[1]);
                        }
                        else
                        {
                            // 单节
                            startN = endN = Convert.ToInt32(nPart);
                        }

                        // 更新起始检索位置
                        start = index + 1;
                        // 查找 , 即课程信息的下一部分
                        index = s.IndexOf(",", start);

                        // 地点
                        string location = "";
                        if (index == -1)
                        {
                            // 没有下一部分，也就是到头了，那这部分全是地点
                            location = s.Substring(start);
                        }
                        else
                        {
                            // 截取地点信息
                            location = s.Substring(start, index - start);
                        }

                        models.Add(new TimeTableItemModel(
                            name: course.Name,
                            description: location + "\n" + course.Teacher,
                            day: day,
                            start: startN,
                            end: endN,
                            course: course
                        ));

                        if (index == -1)
                        {
                            // 没有下一部分，结束
                            break;
                        }

                        // 继续，更新起始检索位置
                        start = index + 1;
                    }
                }

                // 如果这部分周数范围中，不存在指定周
                if (!isIn)
                {
                    // 继续下一部分，更新起始检索位置
                    start = s.IndexOf(",", index + 1) + 1;
                    if (start == 0)
                    {
                        // 没有下一部分，结束
                        break;
                    }
                }

                // 找到下一部分的“周”，以便下一循环开始
                index = s.IndexOf("周", start);
            }

            return models;

        }

        /// <summary>
        /// Parse table courses
        /// </summary>
        /// <param name="tableCourses"></param>
        /// <param name="weekNumber"></param>
        /// <param name="weekCount"></param>
        /// <returns></returns>
        public static TimeTableWeek ParseTableCourses(TableCourses tableCourses, int weekNumber, out int weekCount)
        {
            TimeTableWeek week = new TimeTableWeek("第" + weekNumber + "周");
            weekCount = 0;

            foreach (var course in tableCourses.Table)
            {
                week.Items.AddRange(ParseTableCourse(course, weekNumber, ref weekCount));
            }
            return week;
        }

        public async static Task<TimeTableModel> GenerateTimeTableModel(TableCourses tableCourses)
        {
            Debug.WriteLine("Started: Generate TimeTableModel");
            TimeTableModel model = new TimeTableModel(tableCourses.Name);
            var week1 = ParseTableCourses(tableCourses, 1, out int weekCount);
            model.Weeks.Add(week1);
            for (int i = 2; i <= weekCount; ++i)
            {
                model.Weeks.Add(ParseTableCourses(tableCourses, i, out weekCount));
            }
            model.CurrentWeek = (await GetCurrentWeek()) - 1;
            if (model.CurrentWeek >= weekCount)
                model.CurrentWeek = weekCount - 1;
            if (model.CurrentWeek < 0)
                model.CurrentWeek = 0;
            Debug.WriteLine("Finished: Generate TimeTableModel");
            return model;
        }

        public async static Task<OneDayTimeTableModel> GenerateOneDayTimeTableModel(TableCourses tableCourses)
        {
            OneDayTimeTableModel model = new OneDayTimeTableModel(tableCourses.Name);
            var week = ParseTableCourses(tableCourses, (await GetCurrentWeek()), out int weekCount);
            Debug.WriteLine("Started: Generate One Day TimeTableModel");
            foreach (var item in week.Items)
            {
                if (WEEKS[item.Day - 1] == DateTime.Now.DayOfWeek)
                {
                    model.Courses.Add(item);
                }
            }
            model.Courses.Sort((a, b) => a.Start - b.Start);
            Debug.WriteLine("Finished: Generate One Day TimeTableModel");
            return model;
        }
    }
}
