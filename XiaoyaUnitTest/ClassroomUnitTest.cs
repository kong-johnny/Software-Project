using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VisualStudio.TestTools.UnitTesting.AppContainer;
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
        public async Task TestGetBuildings()
        {
            var buildings = await ClassroomClient.GetBuildings();
            Assert.IsTrue(buildings.Count > 0);
            Assert.IsTrue(buildings[0].Name.Length > 0);
        }
    }
}
