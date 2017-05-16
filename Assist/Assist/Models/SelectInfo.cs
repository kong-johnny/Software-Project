using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xiaoya.Assist.Models
{
    public class SelectInfo
    {
        [JsonProperty]
        public string IsNjActive { get; private set; }
        [JsonProperty]
        public string IsValidTimeRange { get; private set; }
        [JsonProperty]
        public string IsXjls { get; private set; }
        [JsonProperty]
        public string IsYxqxxk { get; private set; }
        [JsonProperty("jssj")]
        public string EndTime { get; private set; }
        [JsonProperty]
        public string Lx { get; private set; }
        [JsonProperty("nj")]
        public string Grade { get; private set; }
        [JsonProperty]
        public string Pycc { get; private set; }
        [JsonProperty]
        public string PyccM { get; private set; }
        [JsonProperty("qssj")]
        public string StartTime { get; private set; }
        [JsonProperty]
        public string Xh { get; private set; }
        [JsonProperty]
        public string XkDesc { get; private set; }
        [JsonProperty]
        public string XkType { get; private set; }
        [JsonProperty("xn")]
        public string Year { get; private set; }
        [JsonProperty]
        public string XnXqDesc { get; private set; }
        [JsonProperty]
        public string XqM { get; private set; }
        [JsonProperty]
        public string XqName { get; private set; }

        [JsonConstructor]
        public SelectInfo(string isNjActive, string isValidTimeRange, string isXjls,
            string isYxqxxk, string endTime, string lx, string grade, string pycc,
            string pyccM, string startTime, string xh, string xkDesc, string xkType,
            string year, string xnXqDesc, string xqM, string xqName)
        {
            IsNjActive       = Convert.ToString(isNjActive).Trim();
            IsValidTimeRange = Convert.ToString(isValidTimeRange).Trim();
            IsXjls           = Convert.ToString(isXjls).Trim();
            IsYxqxxk         = Convert.ToString(isYxqxxk).Trim();
            EndTime          = Convert.ToString(endTime).Trim();
            Lx               = Convert.ToString(lx).Trim();
            Grade            = Convert.ToString(grade).Trim();
            Pycc             = Convert.ToString(pycc).Trim();
            PyccM            = Convert.ToString(pyccM).Trim();
            StartTime        = Convert.ToString(startTime).Trim();
            Xh               = Convert.ToString(xh).Trim();
            XkDesc           = Convert.ToString(xkDesc).Trim();
            XkType           = Convert.ToString(xkType).Trim();
            Year             = Convert.ToString(year).Trim();
            XnXqDesc         = Convert.ToString(xnXqDesc).Trim();
            XqM              = Convert.ToString(xqM).Trim();
            XqName           = Convert.ToString(xqName).Trim();
        }
    }
}
