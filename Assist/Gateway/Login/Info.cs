using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;

namespace srun_login
{
    class Info
    {
        public string username;
        public string password;
        public string ip;
        public string acid;
        public string enc_ver;

        public string ToJsonString()
        {
            var options = new JsonSerializerOptions
            {
                IncludeFields = true
            };
            return JsonSerializer.Serialize(this, options);
        }
    }
}
