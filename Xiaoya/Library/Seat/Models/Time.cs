using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xiaoya.Library.Seat.Models
{
    public class Time
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; private set; }

        [JsonProperty(PropertyName = "value")]
        public string Value { get; private set; }

        [JsonConstructor]
        public Time(string id, string value)
        {
            Id = id;
            Value = value;
        }
    }
}
