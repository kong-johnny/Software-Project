using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xiaoya.Library.Seat.Models
{
    public class Reservation
    {
        [JsonProperty(PropertyName = "id")]
        public int Id { get; private set; }

        [JsonProperty(PropertyName = "receipt")]
        public string Receipt { get; private set; }

        [JsonProperty(PropertyName = "onDate")]
        public string OnDate { get; private set; }

        [JsonProperty(PropertyName = "seatId")]
        public int? SeatId { get; private set; }

        [JsonProperty(PropertyName = "status")]
        public string Status { get; private set; }

        [JsonProperty(PropertyName = "location")]
        public string Location { get; private set; }

        [JsonProperty(PropertyName = "begin")]
        public string Begin { get; private set; }

        [JsonProperty(PropertyName = "end")]
        public string End { get; private set; }

        [JsonProperty(PropertyName = "userEnded")]
        public bool? UserEnded { get; private set; }

        [JsonProperty(PropertyName = "message")]
        public string Message { get; private set; }

        [JsonProperty(PropertyName = "checkedIn")]
        public string CheckedIn { get; private set; }

        public Reservation(int id, string receipt, string onDate, int? seatId,
            string status, string location ,string begin, string end, bool? userEnded,
            string message, string checkedIn)
        {
            Id = id;
            Receipt = receipt;
            OnDate = onDate;
            SeatId = seatId;
            Status = status;
            Location = location;
            Begin = begin;
            End = end;
            UserEnded = userEnded;
            Message = message;
            CheckedIn = checkedIn;
        }
    }
}
