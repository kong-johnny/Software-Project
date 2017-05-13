using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xiaoya.Library.Seat.Models
{
    public class ReservationHistories
    {
        [JsonProperty(PropertyName = "reservations")]
        public List<ReservationHistory> Items { get; private set; }

        [JsonConstructor]
        public ReservationHistories(List<ReservationHistory> items)
        {
            Items = items;
        }
    }
}
