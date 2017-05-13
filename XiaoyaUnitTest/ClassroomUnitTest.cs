using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xiaoya.Classroom;

namespace XiaoyaUnitTest
{
    [TestClass]
    public class ClassroomUnitTest
    {
        [TestMethod]
        public void TestGetBuildings()
        {
            var buildings = ClassroomClient.GetBuildings().Result;
            Assert.IsTrue(buildings.Count > 0);
            Assert.IsTrue(buildings[0].Name.Length > 0);
        }

        [TestMethod]
        public void TestGetRooms()
        {
            ClassroomClient.GetBuildings().ContinueWith(r =>
            {
                ClassroomClient.GetRooms(r.Result[0].Id).ContinueWith(r2 =>
                {
                    var rooms = r2.Result;
                    Assert.IsTrue(rooms.Count > 0);
                    Assert.IsTrue(rooms[0].Name.Length > 0);
                    Assert.AreEqual(rooms[0].HasLecture.Count, 12);
                });
            });
        }
    }
}
