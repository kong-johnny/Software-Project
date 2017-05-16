using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Xiaoya.Library.Seat.Models
{
    public class Reservation : INotifyPropertyChanged
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

        public event PropertyChangedEventHandler PropertyChanged;

        public Reservation Clone()
        {
            return new Reservation(this.Id, this.Receipt, this.OnDate, this.SeatId, this.Status,
                this.Location, this.Begin, this.End, this.UserEnded, this.Message, this.CheckedIn);
        }

        public void Clone(Reservation r)
        {
            Id = r.Id;
            Receipt = r.Receipt;
            OnDate = r.OnDate;
            SeatId = r.SeatId;
            Status = r.Status;
            Location = r.Location;
            Begin = r.Begin;
            End = r.End;
            UserEnded = r.UserEnded;
            Message = r.Message;
            CheckedIn = r.CheckedIn;
            this.PropertyChanged(this, new PropertyChangedEventArgs("Id"));
            this.PropertyChanged(this, new PropertyChangedEventArgs("Receipt"));
            this.PropertyChanged(this, new PropertyChangedEventArgs("OnDate"));
            this.PropertyChanged(this, new PropertyChangedEventArgs("SeatId"));
            this.PropertyChanged(this, new PropertyChangedEventArgs("Status"));
            this.PropertyChanged(this, new PropertyChangedEventArgs("Location"));
            this.PropertyChanged(this, new PropertyChangedEventArgs("Begin"));
            this.PropertyChanged(this, new PropertyChangedEventArgs("End"));
            this.PropertyChanged(this, new PropertyChangedEventArgs("UserEnded"));
            this.PropertyChanged(this, new PropertyChangedEventArgs("Message"));
            this.PropertyChanged(this, new PropertyChangedEventArgs("CheckedIn"));
        }

        [JsonConstructor]
        public Reservation(int id, string receipt, string onDate, int? seatId,
            string status, string location, string begin, string end, bool? userEnded,
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
