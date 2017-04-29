﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xiaoya.Assist.Model
{
    public class GradeInfo
    {

        /// <summary>
        /// Inner class for JSON Parser
        /// </summary>
        public class _GradeInfo
        {
            public string nj, zydm, zymc, pycc, dwh;
        }

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
        /// Constructor
        /// </summary>
        /// <param name="grade">Grade</param>
        /// <param name="major">Major</param>
        /// <param name="majorId">Major Id</param>
        public GradeInfo(string grade, string major, string majorId)
        {
            Grade   = grade.Trim();
            Major   = major.Trim();
            MajorId = majorId.Trim();
        }

        public GradeInfo(_GradeInfo info)
        {
            Grade   = info.nj.Trim();
            Major   = info.zymc.Trim();
            MajorId = info.zydm.Trim();
        }
    }
}
