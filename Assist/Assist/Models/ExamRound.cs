using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xiaoya.Assist.Models
{
    public class ExamRound
    {
        /// <summary>
        /// 考试编码
        /// </summary>
        [JsonProperty]
        public string Code { get; private set; }
        /// <summary>
        /// 考试名称
        /// </summary>
        [JsonProperty]
        public string Name { get; private set; }

        /// <summary>
        /// Calculated year 学年
        /// </summary>
        public string Year { get; private set; }
        /// <summary>
        /// Calculated semester 学期
        /// </summary>
        public string Semester { get; private set; }
        /// <summary>
        /// Calculated round 轮次
        /// </summary>
        public string Round { get; private set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name">Exam name</param>
        /// <param name="code">Exam code</param>
        [JsonConstructor]
        public ExamRound(string name, string code)
        {
            Name = Convert.ToString(name).Trim();
            Code = Convert.ToString(code).Trim();

            // calculate exam year, semester and round
            string[] codeParts = Code.Split(',');
            if (codeParts.Count() == 3)
            {
                Year     = codeParts[0];
                Semester = codeParts[1];
                Round    = codeParts[2];
            }
        }
    }
}
