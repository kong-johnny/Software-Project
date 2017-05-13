using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xiaoya.Library.Seat.Models
{
    public class Room
    {
        /// <summary>
        /// ID
        /// </summary>
        [JsonProperty(PropertyName = "roomId")]
        public int RoomId { get; private set; }

        /// <summary>
        /// Name
        /// </summary>
        [JsonProperty(PropertyName = "room")]
        public string Name { get; private set; }

        /// <summary>
        /// Floor
        /// </summary>
        [JsonProperty(PropertyName = "floor")]
        public string Floor { get; private set; }

        /// <summary>
        /// The number of reserved seats
        /// </summary>
        [JsonProperty(PropertyName = "reserved")]
        public string Reserved { get; private set; }

        /// <summary>
        /// The number of seats in use
        /// </summary>
        [JsonProperty(PropertyName = "inUse")]
        public string InUse { get; private set; }

        /// <summary>
        /// The number of seats whose owner is away
        /// </summary>
        [JsonProperty(PropertyName = "away")]
        public string Away { get; private set; }

        /// <summary>
        /// Total seats
        /// </summary>
        [JsonProperty(PropertyName = "totalSeats")]
        public string TotalSeats { get; private set; }

        /// <summary>
        /// The number of free seats
        /// </summary>
        [JsonProperty(PropertyName = "free")]
        public string Free { get; private set; }

        public Room(int roomId, string name, string floor, string reserved,
            string inUse, string away, string totalSeats, string free)
        {
            RoomId = roomId;
            Name = name;
            Floor = floor;
            Reserved = reserved;
            InUse = inUse;
            Away = away;
            TotalSeats = totalSeats;
            Free = free;
        }
    }
}
