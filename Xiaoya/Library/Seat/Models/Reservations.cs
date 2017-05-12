using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xiaoya.Library.Seat.Models
{
    public class Reservations
    {
        [JsonProperty(PropertyName = "reservations")]
        public List<Reservation> Items { get; private set; }

        [JsonConstructor]
        public Reservations(List<Reservation> items)
        {
            Items = items;
        }
    }
}
