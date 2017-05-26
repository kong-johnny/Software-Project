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

        private const string TEST_USERNAME = "200000000000";

        public string Username { get; set; }
        public string Password { get; set; }

        private CXSession m_Session = new CXSession();

        private string m_Token = "";

        private const string URL_LOGIN = "http://219.224.23.151/rest/auth?username={0}&password={1}";
        private const string URL_HISTORY = "http://219.224.23.151/rest/v2/history/{0}/{1}?token={2}";
        private const string URL_BUILDING = "http://219.224.23.151/rest/v2/free/filters?token={0}";
        private const string URL_ROOM = "http://219.224.23.151/rest/v2/room/stats2/{0}?token={1}";
        private const string URL_CURRENT_RESERVATION = "http://219.224.23.151/rest/v2/user/reservations/?token={0}";
        private const string URL_RESERVATION = "http://219.224.23.151/rest/view/{0}?token={1}";
        private const string URL_CANCEL_RESERVATION = "http://219.224.23.151/rest/v2/cancel/{0}?token={1}";
        private const string URL_LAYOUT = "http://219.224.23.151/rest/v2/room/layoutByDate/{0}/{1}?token={2}";
        private const string URL_START_TIME = "http://219.224.23.151/rest/v2/startTimesForSeat/{0}/{1}?token={2}";
        private const string URL_END_TIME = "http://219.224.23.151/rest/v2/endTimesForSeat/{0}/{1}/{2}?token={3}";
        private const string URL_ORDER = "http://219.224.23.151/rest/v2/freeBook";
        private const string URL_CHECKIN = "http://219.224.23.151/rest/v2/checkIn?token={0}";
        private const string URL_LEAVE = "http://219.224.23.151/rest/v2/leave?token={0}";
        private const string URL_STOP = "http://219.224.23.151/rest/v2/stop?token={0}";

        public SeatClient() { }

        public SeatClient(string username, string password)
        {
            Username = username;
            Password = password;
        }

        public async Task<Result<LoginToken>> Login()
        {
            // TEST

            if (Username == TEST_USERNAME)
            {
                return new Result<LoginToken>("success", new LoginToken("1"), "0", "");
            }

            // TEST

            try
            {
                var res = await m_Session.Req
                    .Url(String.Format(URL_LOGIN, Username, Password))
                    .ClearCookies()
                    .CacheReadBehavior(Windows.Web.Http.Filters.HttpCacheReadBehavior.NoCache)
                    .Get();

                var body = await res.Content();

                var result = JsonConvert.DeserializeObject<Result<LoginToken>>(body);

                m_Token = result.Data.Token;

                return result;
            }
            catch
            {
                return new Result<LoginToken>("failed", null, "", "网络错误");
            }
        }

        public async Task<Result<ReservationHistories>> GetReservationHistory(int start, int end)
        {
            // TEST

            if (Username == TEST_USERNAME)
            {
                return new Result<ReservationHistories>("success", new ReservationHistories(new List<ReservationHistory>()
                {
                    new ReservationHistory("0", "2016-12-30", "19:00", "20:00", "三层自习区", "CANCEL")
                }), "0", "");
            }

            // TEST
            try
            {
                var res = await m_Session.Req
                    .Url(String.Format(URL_HISTORY, start, end, m_Token))
                    .Get();

                var body = await res.Content();

                var result = JsonConvert.DeserializeObject<Result<ReservationHistories>>(body);

                return result;
            }
            catch
            {
                return new Result<ReservationHistories>("failed", null, "", "网络错误");
            }
        }

        public async Task<Result<BuildingInfo>> GetBuildings()
        {
            // TEST

            if (Username == TEST_USERNAME)
            {
                return new Result<BuildingInfo>("success", new BuildingInfo()
                {
                    Buildings = new List<Building>()
                    {
                        new Building()
                        {
                            Id = 0,
                            Name = "主馆",
                            Floor = 3
                        }
                    }
                }, "0", "");
            }

            // TEST
            try
            {
                var res = await m_Session.Req
                    .Url(String.Format(URL_BUILDING, m_Token))
                    .Get();

                var body = await res.Content();

                var result = JsonConvert.DeserializeObject<Result<BuildingInfo>>(body);

                return result;
            }
            catch
            {
                return new Result<BuildingInfo>("failed", null, "", "网络错误");
            }
        }

        public async Task<Result<List<Room>>> GetRooms(int buildingId)
        {
            // TEST

            if (Username == TEST_USERNAME)
            {
                return new Result<List<Room>>("success", new List<Room>()
                {
                    new Room(0, "三层自习区", "3", "10", "3", "1", "20", "6")
                }, "0", "");
            }

            // TEST
            try
            {
                var res = await m_Session.Req
                    .Url(String.Format(URL_ROOM, buildingId, m_Token))
                    .Get();

                var body = await res.Content();

                var result = JsonConvert.DeserializeObject<Result<List<Room>>>(body);

                return result;
            }
            catch
            {
                return new Result<List<Room>>("failed", null, "", "网络错误");
            }
        }

        public async Task<Result<List<Reservation>>> GetCurrentReservation()
        {
            // TEST

            if (Username == TEST_USERNAME)
            {
                return new Result<List<Reservation>>("success", new List<Reservation>()
                {
                    new Reservation(0, "1122-332", "2017-3-12", 0, "RESERVE", "主馆三层自习区112", "19:00", "20:00", false, "", "")
                }, "0", "");
            }

            // TEST
            try
            {
                var res = await m_Session.Req
                    .Url(String.Format(URL_CURRENT_RESERVATION, m_Token))
                    .CacheWriteBehavior(Windows.Web.Http.Filters.HttpCacheWriteBehavior.NoCache)
                    .CacheReadBehavior(Windows.Web.Http.Filters.HttpCacheReadBehavior.NoCache)
                    .Get();

                var body = await res.Content();

                var result = JsonConvert.DeserializeObject<Result<List<Reservation>>>(body);

                return result;
            }
            catch
            {
                return new Result<List<Reservation>>("failed", null, "", "网络错误");
            }
        }

        public async Task<Reservation> GetReservation(int reservationId)
        {
            try
            {
                var res = await m_Session.Req
                    .Url(String.Format(URL_RESERVATION, reservationId, m_Token))
                    .Get();

                var body = await res.Content();

                var result = JsonConvert.DeserializeObject<Reservation>(body);

                return result;
            }
            catch
            {
                return new Reservation(0, "", "网络错误", 0, "", "", "", "", false, "网络错误", "");
            }
        }

        public async Task<Result<string>> CancelReservation(int reservationId)
        {
            // TEST

            if (Username == TEST_USERNAME)
            {
                return new Result<string>("success", "请在图书馆内WIFI完成操作", "0", "请在图书馆内WIFI完成操作");
            }

            // TEST
            try
            {
                var res = await m_Session.Req
                    .Url(String.Format(URL_CANCEL_RESERVATION, reservationId, m_Token))
                    .Get();

                var body = await res.Content();

                var result = JsonConvert.DeserializeObject<Result<string>>(body);

                return result;
            }
            catch
            {
                return new Result<string>("failed", null, "", "网络错误");
            }
        }

        public async Task<Result<SeatLayout>> GetSeatLayout(int roomId, string date)
        {
            // TEST

            if (Username == TEST_USERNAME)
            {
                return new Result<SeatLayout>("success", new SeatLayout(0, "三层自习区", 2, 2, new Dictionary<string, SeatLayoutItem>()
                {
                    {"0000",  new SeatLayoutItem(1, "1", "seat", "FREE", false, false, false, false) },
                    {"0001",  new SeatLayoutItem(2, "2", "seat", "FREE", false, false, false, false) },
                    {"1000",  new SeatLayoutItem(3, "3", "seat", "FREE", false, false, false, false) },
                    {"1001",  new SeatLayoutItem(4, "4", "seat", "FREE", false, false, false, false) },
                }), "0", "");
            }

            // TEST
            try
            {
                var res = await m_Session.Req
                    .Url(String.Format(URL_LAYOUT, roomId, date, m_Token))
                    .Get();

                var body = await res.Content();

                var result = JsonConvert.DeserializeObject<Result<SeatLayout>>(body);

                return result;
            }
            catch
            {
                return new Result<SeatLayout>("failed", null, "", "网络错误");
            }
        }

        public async Task<Result<StartTimes>> GetStartTimes(int seatId, string date)
        {
            // TEST

            if (Username == TEST_USERNAME)
            {
                return new Result<StartTimes>("success", new StartTimes(new List<Time>()
                {
                    new Time("0", "6:00")
                }), "0", "");
            }

            // TEST
            try
            {
                var res = await m_Session.Req
                    .Url(String.Format(URL_START_TIME, seatId, date, m_Token))
                    .Get();

                var body = await res.Content();

                var result = JsonConvert.DeserializeObject<Result<StartTimes>>(body);

                return result;
            }
            catch
            {
                return new Result<StartTimes>("failed", null, "", "网络错误");
            }
        }

        public async Task<Result<EndTimes>> GetEndTimes(int seatId, string date, string startTimeId)
        {
            // TEST

            if (Username == TEST_USERNAME)
            {
                return new Result<EndTimes>("success", new EndTimes(new List<Time>()
                {
                    new Time("0", "10:00")
                }), "0", "");
            }

            // TEST
            try
            {
                var res = await m_Session.Req
                    .Url(String.Format(URL_END_TIME, seatId, date, startTimeId, m_Token))
                    .Get();

                var body = await res.Content();

                var result = JsonConvert.DeserializeObject<Result<EndTimes>>(body);

                return result;
            }
            catch
            {
                return new Result<EndTimes>("failed", null, "", "网络错误");
            }
        }

        public async Task<Result<Reservation>> OrderSeat(int seatId, string date, string startTimeId, string endTimeId)
        {
            // TEST

            if (Username == TEST_USERNAME)
            {
                return new Result<Reservation>("failed", null, "", "请在校园网内操作");
            }

            // TEST
            try
            {
                var res = await m_Session.Req
                    .Url(URL_ORDER)
                    .Data("token", m_Token)
                    .Data("startTime", startTimeId)
                    .Data("endTime", endTimeId)
                    .Data("seat", Convert.ToString(seatId))
                    .Data("date", date)
                    .Post();

                var body = await res.Content();

                var result = JsonConvert.DeserializeObject<Result<Reservation>>(body);

                return result;
            }
            catch
            {
                return new Result<Reservation>("failed", null, "", "网络错误");
            }
        }

        public async Task<Result<string>> CheckIn()
        {
            // TEST

            if (Username == TEST_USERNAME)
            {
                return new Result<string>("success", "请在图书馆内WIFI完成操作", "0", "请在图书馆内WIFI完成操作");
            }

            // TEST
            try
            {
                var res = await m_Session.Req
                    .Url(String.Format(URL_CHECKIN, m_Token))
                    .Get();

                var body = await res.Content();

                var result = JsonConvert.DeserializeObject<Result<string>>(body);

                return result;
            }
            catch
            {
                return new Result<string>("failed", null, "", "网络错误");
            }
        }

        public async Task<Result<string>> Leave()
        {
            // TEST

            if (Username == TEST_USERNAME)
            {
                return new Result<string>("success", "请在图书馆内WIFI完成操作", "0", "请在图书馆内WIFI完成操作");
            }

            // TEST
            try
            {
                var res = await m_Session.Req
                    .Url(String.Format(URL_LEAVE, m_Token))
                    .Get();

                var body = await res.Content();

                var result = JsonConvert.DeserializeObject<Result<string>>(body);

                return result;
            }
            catch
            {
                return new Result<string>("failed", null, "", "网络错误");
            }
        }

        public async Task<Result<string>> Stop()
        {
            // TEST

            if (Username == TEST_USERNAME)
            {
                return new Result<string>("success", "请在图书馆内WIFI完成操作", "0", "请在图书馆内WIFI完成操作");
            }

            // TEST
            try
            {
                var res = await m_Session.Req
                    .Url(String.Format(URL_STOP, m_Token))
                    .Get();

                var body = await res.Content();

                var result = JsonConvert.DeserializeObject<Result<string>>(body);

                return result;
            }
            catch
            {
                return new Result<string>("failed", null, "", "网络错误");
            }
        }
    }
}
