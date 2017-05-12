using CXHttpNS;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xiaoya.Library.Seat.Models;

namespace Xiaoya.Library.Seat
{
    public class SeatClient
    {
        public string Username { get; private set; }
        public string Password { get; private set; }

        private CXSession m_Session = new CXSession();

        private string m_Token = "";

        private const string URL_LOGIN = "http://219.224.23.151/rest/auth?username={0}&password={1}";
        private const string URL_HISTORY = "http://219.224.23.151/rest/v2/history/{0}/{1}?token={2}";
        
        public SeatClient(string username, string password)
        {
            Username = username;
            Password = password;
        }

        public async Task<Result<LoginToken>> Login()
        {
            var res = await m_Session.Req
                .Url(String.Format(URL_LOGIN, Username, Password))
                .Get();

            var body = await res.Content();

            var result = JsonConvert.DeserializeObject<Result<LoginToken>>(body);

            m_Token = result.Data.Token;

            return result;
        }

        public async Task<Result<Reservations>> GetReservationHistory(int start, int end)
        {
            var res = await m_Session.Req
                .Url(String.Format(URL_HISTORY, start, end, m_Token))
                .Get();

            var body = await res.Content();

            var result = JsonConvert.DeserializeObject<Result<Reservations>>(body);

            return result;
        }
    }
}
