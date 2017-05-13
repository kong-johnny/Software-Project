using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xiaoya.Library.Seat.Models
{
    public class EndTimes
    {
        [JsonProperty(PropertyName = "endTimes")]
        public List<Time> Items { get; private set; }

        [JsonConstructor]
        public EndTimes(List<Time> items)
        {
            Items = items;
        }
    }
}
