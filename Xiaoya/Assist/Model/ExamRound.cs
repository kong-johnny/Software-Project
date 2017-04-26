using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xiaoya.Assist.Model
{
    public class ExamRound
    {
        [JsonProperty]
        public string Code { get; private set; }
        [JsonProperty]
        public string Name { get; private set; }

        [JsonConstructor]
        public ExamRound(string name, string code)
        {
            Name = name;
            Code = code;
        }
    }
}
