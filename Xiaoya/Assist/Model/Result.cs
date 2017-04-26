using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xiaoya.Assist.Model
{
    class RequestResult
    {
        [JsonProperty]
        public string Message { get; private set; }
        [JsonProperty]
        public string Status { get; private set; }
        [JsonProperty]
        public string Result { get; private set; }

        RequestResult(string message, string status, string result)
        {
            Message = message;
            Status = status;
            Result = result;
        }
    }
}
