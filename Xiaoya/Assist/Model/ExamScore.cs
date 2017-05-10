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
        /// 课程Id
        /// </summary>
        public string CourseId { get; private set; }
        /// <summary>
        /// 课程学分
        /// </summary>
        public double CourseCredit { get; private set; }
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
        /// 综合名称
        /// </summary>
        public string ComplexName { get => CourseName + (IsMajor ? "" : " 辅修") + (DoLearnForFirstTime ? "" : " 重修"); }
        
        /// <summary>
        /// 是否为公共课
        /// </summary>
        public bool IsMajorCourse { get => Classification.Contains("专业") || Classification.Contains("院系"); }

        /// <summary>
        /// 数值化最终成绩
        /// </summary>
        public double NumericScore
        {
            get
            {
                if (!Double.TryParse(Score, out double s))
                {
                    s = 85.0;
                }
                return s;
            }
        }

        /// <summary>
        /// 标准4分制GPA
        /// </summary>
        public double StandardFourPointsGPA
        {
            get
            {
                double s = NumericScore;
                if (s >= 90 && s <= 100) return 4.0;
                if (s >= 80 && s <= 89) return 3.0;
                if (s >= 70 && s <= 79) return 2.0;
                if (s >= 60 && s <= 69) return 1.0;
                return 0.0;
            }
        }

        /// <summary>
        /// 改进(1)4分制GPA
        /// </summary>
        public double ImprovedFourPointsGPA1
        {
            get
            {
                double s = NumericScore;
                if (s >= 85 && s <= 100) return 4.0;
                if (s >= 70 && s <= 84) return 3.0;
                if (s >= 60 && s <= 69) return 2.0;
                return 0.0;
            }
        }

        /// <summary>
        /// 改进(2)4分制GPA
        /// </summary>
        public double ImprovedFourPointsGPA2
        {
            get
            {
                double s = NumericScore;
                if (s >= 85 && s <= 100) return 4.0;
                if (s >= 75 && s <= 84) return 3.0;
                if (s >= 60 && s <= 74) return 2.0;
                return 0.0;
            }
        }

        /// <summary>
        /// 北大4分制GPA
        /// </summary>
        public double PKUFourPointsGPA
        {
            get
            {
                double s = NumericScore;
                if (s >= 90 && s <= 100) return 4.0;
                if (s >= 85 && s <= 89) return 3.7;
                if (s >= 82 && s <= 84) return 3.3;
                if (s >= 78 && s <= 81) return 3.0;
                if (s >= 75 && s <= 77) return 2.7;
                if (s >= 72 && s <= 74) return 2.3;
                if (s >= 68 && s <= 71) return 2.0;
                if (s >= 64 && s <= 67) return 1.5;
                if (s >= 60 && s <= 63) return 1.0;
                return 0.0;
            }
        }


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
            Semester            = Convert.ToString(semester).Trim();
            CourseName          = Convert.ToString(courseName).Trim();
            CourseCredit        = Convert.ToDouble(Convert.ToString(courseCredit).Trim());
            Classification      = Convert.ToString(classification).Trim();
            Score1              = Convert.ToString(score1).Trim();
            Score2              = Convert.ToString(score2).Trim();
            Score               = Convert.ToString(score).Trim();
            DoLearnForFirstTime = doLearnForFirstTime;
            IsMajor             = isMajor;

            if (CourseName.Contains("]"))
            {
                CourseId = CourseName.Substring(1, CourseName.IndexOf("]") - 1);
                CourseName = CourseName.Substring(CourseName.IndexOf("]") + 1);
            }
        }
    }
}
