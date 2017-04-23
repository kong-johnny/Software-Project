using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xiaoya.Assist.Model
{
    public class GradeInfo
    {

        public class _GradeInfo
        {
            public string nj, zydm, zymc, pycc, dwh;
        }

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

        public GradeInfo(_GradeInfo info)
        {
            m_Grade = info.nj;
            m_Major = info.zymc;
            m_MajorId = info.zydm;
        }
    }
}
