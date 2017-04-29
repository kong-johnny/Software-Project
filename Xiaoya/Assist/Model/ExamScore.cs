using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xiaoya.Assist.Model
{
    public class ExamScore
    {
        /// <summary>
        /// 学期
        /// </summary>
        public string Semester { get; private set; }
        /// <summary>
        /// 课程名称
        /// </summary>
        public string CourseName { get; private set; }
        /// <summary>
        /// 课程学分
        /// </summary>
        public string CourseCredit { get; private set; }
        /// <summary>
        /// 课程类别
        /// </summary>
        public string Classification { get; private set; }
        /// <summary>
        /// 平时成绩
        /// </summary>
        public string Score1 { get; private set; }
        /// <summary>
        /// 期末成绩
        /// </summary>
        public string Score2 { get; private set; }
        /// <summary>
        /// 最终成绩
        /// </summary>
        public string Score { get; private set; }
        /// <summary>
        /// 是否初修
        /// </summary>
        public bool DoLearnForFirstTime { get; private set; }
        /// <summary>
        /// 是否主修
        /// </summary>
        public bool IsMajor { get; private set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="semester">Semester</param>
        /// <param name="courseName">Course name</param>
        /// <param name="courseCredit">Course credit</param>
        /// <param name="classification">Course classification</param>
        /// <param name="score1">Score for usual performance</param>
        /// <param name="score2">Score for final exam</param>
        /// <param name="score">Final score</param>
        /// <param name="doLearnForFirstTime">Whether student learned this course for the first time</param>
        /// <param name="isMajor">Whether this course is the student's major course</param>
        public ExamScore(string semester, string courseName, string courseCredit,
            string classification, string score1, string score2, string score,
            bool doLearnForFirstTime, bool isMajor)
        {
            Semester            = semester.Trim();
            CourseName          = courseName.Trim();
            CourseCredit        = courseCredit.Trim();
            Classification      = classification.Trim();
            Score1              = score1.Trim();
            Score2              = score2.Trim();
            Score               = score.Trim();
            DoLearnForFirstTime = doLearnForFirstTime;
            IsMajor             = isMajor;
        }
    }
}
