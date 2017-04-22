using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xiaoya.Assist.Model
{
    public class GradeInfo
    {
        private string m_Grade, m_Major, m_MajorId;

        public string Grade { get => m_Grade; }
        public string Major { get => m_Major; }
        public string MajorId { get => m_MajorId; }

        public GradeInfo(string grade, string major, string majorId)
        {
            m_Grade = grade;
            m_Major = major;
            m_MajorId = majorId;
        }
    }
}
