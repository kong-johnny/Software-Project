using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xiaoya;
using Xiaoya.Helpers;

namespace XiaoyaUnitTest
{
    [TestClass]
    public class HelperUnitTest
    {
        Assistant assist = new Assistant();

        async Task LoginAssist()
        {
            var info = File.ReadLines("Assist.txt").ToList();
            assist.Username = info[0];
            assist.Password = info[1];
            var loginRes = await assist.Login();
            Assert.IsNull(loginRes);
        }

        [TestMethod]
        public async Task TestGetCurrentWeek()
        {
            var week = await TimeTableHelper.GetCurrentWeek();
            Assert.IsTrue(week >= 1);
        }

        [TestMethod]
        public async Task TestGenerateTimeTableModel()
        {
            await LoginAssist();
            var table = await assist.GetTableCourses(assist.GetTableSemesters().Result[0]);
            var model = await TimeTableHelper.GenerateTimeTableModel(table);
            Assert.IsTrue(model.Weeks.Count > 15);
            Assert.IsTrue(model.Weeks[0].Items.Count > 0);
            Assert.IsTrue(model.Weeks[0].Items[0].Name.Length > 0);
        }

        [TestMethod]
        public async Task TestGenerateOneDayTimeTableModel()
        {
            await LoginAssist();
            var table = await assist.GetTableCourses(assist.GetTableSemesters().Result[0]);
            var model = await TimeTableHelper.GenerateOneDayTimeTableModel(table);
            Assert.IsTrue(model.Courses.Count >= 0);
            if (model.Courses.Count > 0)
                Assert.IsTrue(model.Courses[0].Name.Length > 0);
        }
    }
}
