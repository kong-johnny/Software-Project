using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xiaoya.Library.Seat.Models
{
    public class SeatLayoutItem
    {
        [JsonProperty(PropertyName = "id")]
        public int? Id { get; private set; }

        [JsonProperty(PropertyName = "name")]
        public string Name { get; private set; }

        [JsonProperty(PropertyName = "type")]
        public string Type { get; private set; }

        [JsonProperty(PropertyName = "status")]
        public string Status { get; private set; }

        public string ShowStatus
        {
            get
            {
                switch (Status)
                {
                    case "AWAY":
                        return "暂离";
                    case "IN_USE":
                        return "使用中";
                    case "FREE":
                        return "空闲";
                    case "BOOKED":
                        return "已预订";
                    default:
                        return Status;
                }
            }
        }
        
        [JsonProperty(PropertyName = "window")]
        public bool? Window { get; private set; }

        [JsonProperty(PropertyName = "power")]
        public bool? Power { get; private set; }

        [JsonProperty(PropertyName = "computer")]
        public bool? Computer { get; private set; }

        [JsonProperty(PropertyName = "local")]
        public bool? Local { get; private set; }

        [JsonConstructor]
        public SeatLayoutItem(int? id, string name, string type,
            string status, bool? window, bool? power, bool? computer, bool? local)
        {
            Id = id;
            Name = name;
            Type = type;
            Status = status;
            Window = window;
            Power = power;
            Computer = computer;
            Local = local;
        }
    }
}
