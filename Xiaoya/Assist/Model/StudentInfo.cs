using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xiaoya.Assist.Model
{
    public class StudentInfo
    {

        public string StudentId { get; private set; }
        public string Grade { get; private set; }
        public string Major { get; private set; }
        public string MajorId { get; private set; }
        public string SchoolYear { get; private set; }
        public string Semester { get; private set; }

        public StudentInfo(string studentId, string grade, string major, string majorId, string schoolYear, string semester)
        {
            StudentId = studentId;
            Grade = grade;
            Major = major;
            MajorId = majorId;
            SchoolYear = schoolYear;
            Semester = semester;
        }
    }
}
