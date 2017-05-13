using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xiaoya.Library.Seat.Models
{
    public class SeatLayout
    {
        [JsonProperty(PropertyName = "id")]
        public int Id { get; private set; }

        [JsonProperty(PropertyName = "name")]
        public string Name{ get; private set; }

        [JsonProperty(PropertyName = "cols")]
        public int Columns { get; private set; }

        [JsonProperty(PropertyName = "rows")]
        public int Rows { get; private set; }

        [JsonProperty(PropertyName = "layout")]
        public Dictionary<string, SeatLayoutItem> Layout { get; private set; }

        public SeatLayout(int id, string name, int cols, int rows,
            Dictionary<string, SeatLayoutItem> layout)
        {
            Id = id;
            Name = name;
            Columns = cols;
            Rows = rows;
            Layout = layout;
        }
    }
}
