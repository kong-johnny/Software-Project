using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xiaoya.Assist.Model
{
    public class StudentInfo
    {
        /// <summary>
        /// 内部学号
        /// </summary>
        public string StudentId { get; private set; }
        /// <summary>
        /// 年级
        /// </summary>
        public string Grade { get; private set; }
        /// <summary>
        /// 专业
        /// </summary>
        public string Major { get; private set; }
        /// <summary>
        /// 专业Id
        /// </summary>
        public string MajorId { get; private set; }
        /// <summary>
        /// 学年
        /// </summary>
        public string SchoolYear { get; private set; }
        /// <summary>
        /// 学期
        /// </summary>
        public string Semester { get; private set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="studentId">Student Id</param>
        /// <param name="grade">Grade</param>
        /// <param name="major">Major</param>
        /// <param name="majorId">Major Id</param>
        /// <param name="schoolYear">School year</param>
        /// <param name="semester">Semester</param>
        public StudentInfo(string studentId, string grade, string major, string majorId, string schoolYear, string semester)
        {
            StudentId  = studentId.Trim();
            Grade      = grade.Trim();
            Major      = major.Trim();
            MajorId    = majorId.Trim();
            SchoolYear = schoolYear.Trim();
            Semester   = semester.Trim();
        }
    }
}
