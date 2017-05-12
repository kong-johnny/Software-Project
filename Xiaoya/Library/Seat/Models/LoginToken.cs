using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xiaoya.Library.Seat.Models
{
    public class LoginToken
    {
        [JsonProperty(PropertyName = "token")]
        public string Token { get; private set; }

        [JsonConstructor]
        public LoginToken(string token)
        {
            Token = token;
        }
    }
}
