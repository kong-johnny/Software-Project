using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xiaoya.Gateway.Models
{
    public class GatewayUser
    {
        public string Username { get; set; }
        public string Password { get; set; }

        public GatewayUser(string usnm, string pwd)
        {
            Username = usnm;
            Password = pwd;
        }
    }
}
