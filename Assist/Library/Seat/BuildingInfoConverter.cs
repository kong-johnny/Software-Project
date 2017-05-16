using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xiaoya.Library.Seat.Models;

namespace Xiaoya.Library.Seat
{
    public class BuildingInfoConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(Building);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
                return null;
            var array = JArray.Load(reader);
            var item = (existingValue as Building ?? new Building());
            item.Id = (int)array.ElementAtOrDefault(0);
            item.Name = (string)array.ElementAtOrDefault(1);
            item.Floor = (int)array.ElementAtOrDefault(2);
            return item;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var item = (Building)value;
            serializer.Serialize(writer, new object[] { item.Id, item.Name, item.Floor });
        }
    }
}
