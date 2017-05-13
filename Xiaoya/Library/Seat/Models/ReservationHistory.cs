using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xiaoya.Library.Seat.Models
{
    public class ReservationHistory
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; private set; }
        [JsonProperty(PropertyName = "date")]
        public string Date { get; private set; }
        [JsonProperty(PropertyName = "begin")]
        public string Begin { get; private set; }
        [JsonProperty(PropertyName = "end")]
        public string End { get; private set; }
        [JsonProperty(PropertyName = "loc")]
        public string Location { get; private set; }
        [JsonProperty(PropertyName = "stat")]
        public string State { get; private set; }

        [JsonConstructor]
        public ReservationHistory(string id, string date, string begin, string end, 
            string location, string state)
        {
            Id = id;
            Date = date;
            Begin = begin;
            End = end;
            Location = location;
            State = state;
        }
    }
}
