using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xiaoya.Library.Seat.Models
{
    public class StartTimes
    {
        [JsonProperty(PropertyName = "startTimes")]
        public List<Time> Items { get; private set; }

        [JsonConstructor]
        public StartTimes(List<Time> items)
        {
            Items = items;
        }
    }
}
