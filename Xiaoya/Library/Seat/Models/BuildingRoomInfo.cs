using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xiaoya.Library.Seat.Models
{
    public class BuildingInfo
    {
        [JsonProperty(PropertyName = "buildings", ItemConverterType = typeof(BuildingInfoConverter))]
        public List<Building> Buildings { get; private set; }
    }
}
