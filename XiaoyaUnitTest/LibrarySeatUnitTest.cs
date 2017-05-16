
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xiaoya.Library.Seat;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace XiaoyaUnitTest
{
    [TestClass]
    public class LibrarySeatUnitTest
    {
        SeatClient client = new SeatClient();

        async Task Login()
        {
            var info = File.ReadLines("Library.txt").ToList();
            client.Username = info[0];
            client.Password = info[1];

            var res = await client.Login();
            Assert.IsNotNull(res);
            Assert.IsNotNull(res.Data);
            Assert.IsNotNull(res.Data.Token);
            Assert.IsTrue(res.Data.Token.Length > 0);

            Console.WriteLine(res.Data.Token);

        }

        [TestMethod]
        public async Task TestLogin()
        {
            await Login();
        }

        [TestMethod]
        public async Task TestGetHistory()
        {
            await Login();
            var res = await client.GetReservationHistory(1, 30);
            Assert.IsNotNull(res);
            Assert.IsNotNull(res.Data);
            Assert.IsNotNull(res.Data.Items);
            Assert.IsTrue(res.Data.Items.Count > 0);
            Assert.IsNotNull(res.Data.Items[0].Location);
            Assert.IsTrue(res.Data.Items[0].Location.Length > 0);
        }

        [TestMethod]
        public async Task TestGetBuilding()
        {
            await Login();
            var res = await client.GetBuildings();
            Assert.IsNotNull(res);
            Assert.IsNotNull(res.Data);
            Assert.IsNotNull(res.Data.Buildings);
            Assert.IsTrue(res.Data.Buildings.Count > 0);
            Assert.IsNotNull(res.Data.Buildings[0].Name);
            Assert.IsTrue(res.Data.Buildings[0].Name.Length > 0);
        }

        [TestMethod]
        public async Task TestGetRoom()
        {
            await Login();
            var res = await client.GetBuildings();
            var rooms = await client.GetRooms(res.Data.Buildings[0].Id);
            Assert.IsNotNull(rooms.Data);
            Assert.IsTrue(rooms.Data.Count > 0);
            Assert.IsNotNull(rooms.Data[0].Name);
            Assert.IsTrue(rooms.Data[0].Name.Length > 0);
        }

        [TestMethod]
        public async Task TestGetCurrentReservation()
        {
            await Login();
            var res = await client.GetCurrentReservation();
            if (res.Data != null)
            {
                Assert.IsNotNull(res.Data.Count > 0);
                Assert.IsNotNull(res.Data[0].Message);
                Assert.IsTrue(res.Data[0].Message.Length > 0);
            }
        }

        [TestMethod]
        public async Task TestGetReservation()
        {
            await Login();
            var res = await client.GetReservation(911777);
            Assert.IsNotNull(res.Id);
            Assert.AreEqual(res.Id, 911777);
        }

        [TestMethod]
        public async Task TestCancelReservation()
        {
            await Login();
            var res = await client.CancelReservation(911777);
            Assert.IsNull(res.Data);
            Assert.IsTrue(res.Code == "0" || res.Code == "1");
        }

        [TestMethod]
        public async Task TestGetSeatLayout()
        {
            await Login();
            var res = await client.GetSeatLayout(
                client.GetRooms(
                    client.GetBuildings().Result.Data.Buildings[0].Id
                ).Result.Data[0].RoomId,
                "2017-5-1"
            );
            Assert.IsNotNull(res.Data.Layout);
            Assert.IsTrue(res.Data.Layout.Count > 0);
            Assert.IsNotNull(res.Data.Layout["0"]);
            Assert.IsNotNull(res.Data.Layout["0"].Type);
            Assert.IsTrue(res.Data.Layout["0"].Type.Length > 0);
        }
    }
}
