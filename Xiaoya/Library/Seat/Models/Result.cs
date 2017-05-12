using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xiaoya.Library.Seat.Models
{
    public class Result<T>
    {
        [JsonProperty(PropertyName = "status")]
        public string Status { get; private set; }

        [JsonProperty(PropertyName = "data")]
        public T Data{ get; private set; }

        [JsonProperty(PropertyName = "code")]
        public string Code{ get; private set; }

        [JsonProperty(PropertyName = "message")]
        public string Message{ get; private set; }

        [JsonConstructor]
        public Result(string status, T data, string code, string message)
        {
            Status = status;
            Data = data;
            Code = code;
            Message = message;
        }
    }
}
