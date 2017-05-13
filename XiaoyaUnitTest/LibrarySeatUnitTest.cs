
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xiaoya.Library.Seat;
using System.IO;
using System.Linq;

namespace XiaoyaUnitTest
{
    [TestClass]
    public class LibrarySeatUnitTest
    {
        SeatClient client = new SeatClient();

        void Login()
        {
            var info = File.ReadLines("Library.txt").ToList();
            client.Username = info[0];
            client.Password = info[1];

            var res = client.Login().Result;

            Assert.IsNotNull(res);
            Assert.IsNotNull(res.Data);
            Assert.IsNotNull(res.Data.Token);
            Assert.IsTrue(res.Data.Token.Length > 0);

            Console.WriteLine(res.Data.Token);
        }

        [TestMethod]
        public void TestLogin()
        {
            Login();
        }

        [TestMethod]
        public void TestGetHistory()
        {
            Login();
            var res = client.GetReservationHistory(1, 30).Result;
            Assert.IsNotNull(res);
            Assert.IsNotNull(res.Data);
            Assert.IsNotNull(res.Data.Items);
            Assert.IsTrue(res.Data.Items.Count > 0);
            Assert.IsNotNull(res.Data.Items[0].Location);
            Assert.IsTrue(res.Data.Items[0].Location.Length > 0);
        }

        [TestMethod]
        public void TestGetBuilding()
        {
            Login();
            var res = client.GetBuildings().Result;
            Assert.IsNotNull(res);
            Assert.IsNotNull(res.Data);
            Assert.IsNotNull(res.Data.Buildings);
            Assert.IsTrue(res.Data.Buildings.Count > 0);
            Assert.IsNotNull(res.Data.Buildings[0].Name);
            Assert.IsTrue(res.Data.Buildings[0].Name.Length > 0);
        }

        [TestMethod]
        public void TestGetRoom()
        {
            Login();
            var res = client.GetBuildings().Result;
            var rooms = client.GetRooms(res.Data.Buildings[0].Id).Result;
            Assert.IsNotNull(rooms.Data);
            Assert.IsTrue(rooms.Data.Count > 0);
            Assert.IsNotNull(rooms.Data[0].Name);
            Assert.IsTrue(rooms.Data[0].Name.Length > 0);
        }

        [TestMethod]
        public void TestGetCurrentReservation()
        {
            Login();
            var res = client.GetCurrentReservation().Result;

            if (res.Data != null)
            {
                Assert.IsNotNull(res.Data.Count > 0);
                Assert.IsNotNull(res.Data[0].Message);
                Assert.IsTrue(res.Data[0].Message.Length > 0);
            }
        }

        [TestMethod]
        public void TestGetReservation()
        {
            Login();
            var res = client.GetReservation(911777).Result;

            Assert.IsNotNull(res.Id);
            Assert.AreEqual(res.Id, 911777);
        }

        [TestMethod]
        public void TestCancelReservation()
        {
            Login();
            var res = client.CancelReservation(911777).Result;
            Assert.IsNull(res.Data);
            Assert.IsTrue(res.Code == "0" || res.Code == "1");
        }

        [TestMethod]
        public void TestGetSeatLayout()
        {
            Login();
            var res = client.GetSeatLayout(
                client.GetRooms(
                    client.GetBuildings().Result.Data.Buildings[0].Id
                ).Result.Data[0].RoomId,
                "2017-5-1"
            ).Result;
            Assert.IsNotNull(res.Data.Layout);
            Assert.IsTrue(res.Data.Layout.Count > 0);
            Assert.IsNotNull(res.Data.Layout["0"]);
            Assert.IsNotNull(res.Data.Layout["0"].Type);
            Assert.IsTrue(res.Data.Layout["0"].Type.Length > 0);
        }

        [TestMethod]
        public void TestGetStartEndTimes()
        {
            Login();
            var res = client.GetStartTimes(48954, "2017-5-13").Result;

            Assert.IsNotNull(res.Data);
            Assert.IsNotNull(res.Data.Items);
            if (res.Data.Items.Count > 0)
            {
                Assert.IsNotNull(res.Data.Items[0].Id);
                Assert.IsTrue(res.Data.Items[0].Id.Length > 0);
            }

            var res2 = client.GetEndTimes(48954, "2017-5-13", "660").Result;

            Assert.IsNotNull(res2.Data);
            Assert.IsNotNull(res2.Data.Items);
            if (res2.Data.Items.Count > 0)
            {
                Assert.IsNotNull(res2.Data.Items[0].Id);
                Assert.IsTrue(res2.Data.Items[0].Id.Length > 0);
            }
        }
    }
}
