using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xiaoya.Assist.Model
{
    public class ExamScore
    {
        public string Term { get; private set; }
        public string CourseName { get; private set; }
        public string CourseCredit { get; private set; }
        public string Classification { get; private set; }
        public string Score1 { get; private set; }
        public string Score2 { get; private set; }
        public string Score { get; private set; }
        public bool DoLearnForFirstTime { get; private set; }
        public bool IsMajor { get; private set; }

        public ExamScore(string term, string courseName, string courseCredit,
            string classification, string score1, string score2, string score,
            bool doLearnForFirstTime, bool isMajor)
        {
            Term = term;
            CourseName = courseName;
            CourseCredit = courseCredit;
            Classification = classification;
            Score1 = score1;
            Score2 = score2;
            Score = score;
            DoLearnForFirstTime = doLearnForFirstTime;
            IsMajor = isMajor;
        }
    }
}
