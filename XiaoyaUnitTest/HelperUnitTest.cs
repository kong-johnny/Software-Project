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

        void LoginAssist()
        {
            var info = File.ReadLines("Assist.txt").ToList();
            assist.Username = info[0];
            assist.Password = info[1];
            var loginRes = assist.Login().Result;
            Assert.IsNull(loginRes);
        }

        [TestMethod]
        public void TestGetCurrentWeek()
        {
            var week = TimeTableHelper.GetCurrentWeek().Result;
            Assert.IsTrue(week >= 1);
        }

        [TestMethod]
        public void TestGenerateTimeTableModel()
        {
            LoginAssist();
            var table = assist.GetTableCourses(assist.GetTableSemesters().Result[0]).Result;
            var model = TimeTableHelper.GenerateTimeTableModel(table).Result;
            Assert.IsTrue(model.Weeks.Count > 15);
            Assert.IsTrue(model.Weeks[0].Items.Count > 0);
            Assert.IsTrue(model.Weeks[0].Items[0].Name.Length > 0);
        }

        [TestMethod]
        public void TestGenerateOneDayTimeTableModel()
        {
            LoginAssist();
            var table = assist.GetTableCourses(assist.GetTableSemesters().Result[0]).Result;
            var model = TimeTableHelper.GenerateOneDayTimeTableModel(table).Result;
            Assert.IsTrue(model.Courses.Count >= 0);
            if (model.Courses.Count > 0)
                Assert.IsTrue(model.Courses[0].Name.Length > 0);
        }
    }
}
