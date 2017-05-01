using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xiaoya.Assist.Model
{
    public class ExamArrangement
    {
        /// <summary>
        /// 课程名称
        /// </summary>
        public string CourseName { get; private set; }
        /// <summary>
        /// 学分
        /// </summary>
        public string Credit { get; private set; }
        /// <summary>
        /// 类别
        /// </summary>
        public string Classification { get; private set; }
        /// <summary>
        /// 考核方式
        /// </summary>
        public string ExamType { get; private set; }
        /// <summary>
        /// 考试时间
        /// </summary>
        public string Time { get; private set; }
        /// <summary>
        /// 考试地点
        /// </summary>
        public string Location { get; private set; }
        /// <summary>
        /// 座位号
        /// </summary>
        public string Seat { get; private set; }

        /// <summary>
        /// Calulated begin time 
        /// </summary>
        public DateTime? BeginTime { get; private set; }
        /// <summary>
        /// Calulated end time 
        /// </summary>
        public DateTime? EndTime { get; private set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="courseName">Course name</param>
        /// <param name="credit">Course credit</param>
        /// <param name="classification">Course classification</param>
        /// <param name="examType">Exam type</param>
        /// <param name="time">Exam time</param>
        /// <param name="location">Exam location</param>
        /// <param name="seat">Exam seat</param>
        public ExamArrangement(string courseName, string credit, string classification,
            string examType, string time, string location, string seat)
        {
            CourseName     = Convert.ToString(courseName).Trim();
            Credit         = Convert.ToString(credit).Trim();
            Classification = Convert.ToString(classification).Trim();
            ExamType       = Convert.ToString(examType).Trim();
            Time           = Convert.ToString(time).Trim();
            Location       = Convert.ToString(location).Trim();
            Seat           = Convert.ToString(seat).Trim();
            BeginTime      = null;
            EndTime        = null;

            // throw [...] in the course name
            if (CourseName.Contains("]"))
            {
                CourseName = CourseName.Substring(CourseName.IndexOf("]") + 1);
            }

            // calculate begin time and end time.
            if (Time.Contains("(") && Time.Contains(")") && Time.Contains("-"))
            {
                string[] datePart = Time.Substring(0, Time.IndexOf("(")).Split('-');
                string[] timePart = Time.Substring(Time.IndexOf(")") + 1).Split('-');
                if (timePart.Count() == 2)
                {
                    string[] begin = timePart[0].Split(':');
                    string[] end = timePart[1].Split(':');
                    BeginTime = new DateTime(
                        Convert.ToInt32(datePart[0]),
                        Convert.ToInt32(datePart[1]),
                        Convert.ToInt32(datePart[2]),
                        Convert.ToInt32(begin[0]),
                        Convert.ToInt32(begin[1]),
                        0
                    );
                    EndTime = new DateTime(
                        Convert.ToInt32(datePart[0]),
                        Convert.ToInt32(datePart[1]),
                        Convert.ToInt32(datePart[2]),
                        Convert.ToInt32(end[0]),
                        Convert.ToInt32(end[1]),
                        0
                    );
                }
            }
        }
    }
}
