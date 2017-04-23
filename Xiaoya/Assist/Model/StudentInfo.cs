using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xiaoya.Assist.Model
{
    public class StudentInfo
    {

        private string m_StudentId, m_Grade, m_Major, m_MajorId, m_SchoolYear, m_Semester;

        public string StudentId { get => m_StudentId; }
        public string Grade { get => m_Grade; }
        public string Major { get => m_Major; }
        public string MajorId { get => m_MajorId; }
        public string SchoolYear { get => m_SchoolYear; }
        public string Semester { get => m_Semester; }

        public StudentInfo(string studentId, string grade, string major, string majorId, string schoolYear, string semester)
        {
            m_StudentId = studentId;
            m_Grade = grade;
            m_Major = major;
            m_MajorId = majorId;
            m_SchoolYear = schoolYear;
            m_Semester = semester;
        }
    }
}
