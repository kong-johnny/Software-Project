using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xiaoya.Assist.Models
{
    class RequestResult
    {
        /// <summary>
        /// 消息
        /// </summary>
        [JsonProperty]
        public string Message { get; private set; }
        /// <summary>
        /// 状态码
        /// </summary>
        [JsonProperty]
        public string Status { get; private set; }
        /// <summary>
        /// 结果
        /// </summary>
        [JsonProperty]
        public string Result { get; private set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="message">Message</param>
        /// <param name="status">Status code</param>
        /// <param name="result">Result</param>
        [JsonConstructor]
        RequestResult(string message, string status, string result)
        {
            Message = Convert.ToString(message).Trim();
            Status  = Convert.ToString(status).Trim();
            Result  = Convert.ToString(result).Trim();
        }
    }
}
