using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xiaoya.Assist.Model
{
    public class TableCourse
    {
        /// <summary>
        /// 课程代码
        /// </summary>
        [JsonProperty(PropertyName = "code")]
        public string Code { get; private set; }
        /// <summary>
        /// 课程名称
        /// </summary>
        [JsonProperty(PropertyName = "name")]
        public string Name { get; private set; }
        /// <summary>
        /// 学分
        /// </summary>
        [JsonProperty(PropertyName = "credit")]
        public string Credit { get; private set; }
        /// <summary>
        /// 任课教师
        /// </summary>
        [JsonProperty(PropertyName = "teacher")]
        public string Teacher { get; private set; }
        /// <summary>
        /// 任课教师Id
        /// </summary>
        [JsonProperty(PropertyName = "teacherId")]
        public string TeacherId { get; private set; }
        /// <summary>
        /// 时间地点
        /// </summary>
        [JsonProperty(PropertyName = "locationTime")]
        public string LocationTime { get; private set; }
        /// <summary>
        /// 是否免听
        /// </summary>
        [JsonProperty(PropertyName = "isFreeToListen")]
        public bool IsFreeToListen { get; private set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="code">Course code</param>
        /// <param name="name">Course name</param>
        /// <param name="credit">Course credit</param>
        /// <param name="teacher">Teacher for the course</param>
        /// <param name="locationTime">Location and time for the course</param>
        /// <param name="isFreeToListen">Is free to listen this course for current student</param>
        [JsonConstructor]
        public TableCourse(string code, string name, string credit, string teacher,
            string locationTime, bool isFreeToListen)
        {
            Code           = Convert.ToString(code).Trim();
            Name           = Convert.ToString(name).Trim();
            Credit         = Convert.ToString(credit).Trim();
            Teacher        = Convert.ToString(teacher).Trim();
            LocationTime   = Convert.ToString(locationTime).Trim();
            IsFreeToListen = isFreeToListen;

            if (Name.Contains("]"))
            {
                Name = Name.Substring(Name.IndexOf("]") + 1);
            }

            if (Teacher.Contains("]"))
            {
                TeacherId = Teacher.Substring(1, Teacher.IndexOf("]") - 1);
                Teacher = Teacher.Substring(Teacher.IndexOf("]") + 1);
            }
        }
    }
}
