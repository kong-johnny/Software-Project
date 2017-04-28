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

        public string Grade { get; private set; }
        public string Major { get; private set; }
        public string MajorId { get; private set; }

        public GradeInfo(string grade, string major, string majorId)
        {
            Grade = grade;
            Major = major;
            MajorId = majorId;
        }

        public GradeInfo(_GradeInfo info)
        {
            Grade = info.nj;
            Major = info.zymc;
            MajorId = info.zydm;
        }
    }
}
