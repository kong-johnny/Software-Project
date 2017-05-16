using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xiaoya;

namespace XiaoyaUnitTest
{
    [TestClass]
    public class AssistUnitTest
    {
        Assistant assist = new Assistant();

        async Task Login()
        {
            var info = File.ReadLines("Assist.txt").ToList();
            assist.Username = info[0];
            assist.Password = info[1];
            var loginRes = await assist.Login();
            Assert.IsNull(loginRes);
        }

        [TestMethod]
        public async Task TestLogin()
        {
            await Login();
        }

        [TestMethod]
        public async Task TestGetStudentInfo()
        {
            await Login();
            var res = await assist.GetStudentInfo();
            Assert.AreEqual(res.Grade, assist.Username.Substring(0, 4));
        }

        [TestMethod]
        public async Task TestGetStudentDetails()
        {
            await Login();
            var res = await assist.GetStudentDetails();
            Assert.IsTrue(res.College.Length > 0);
        }

        [TestMethod]
        public async Task TestGetExamScores()
        {
            await Login();
            var res = await assist.GetExamScores(true);
            Assert.IsTrue(res.Count > 0);
            Assert.IsTrue(res[0].CourseName.Length > 0);
        }

        [TestMethod]
        public async Task TestGetExamArragement()
        {
            await Login();
            var rounds = await assist.GetExamRounds();
            Assert.IsTrue(rounds.Count > 0);
            var arrangement = await assist.GetExamArrangement(rounds[0]);
            Assert.IsTrue(arrangement.Count > 0);
            Assert.IsTrue(arrangement[0].CourseName.Length > 0);
        }

        [TestMethod]
        public async Task TestGetTableCourses()
        {
            await Login();
            var semesters = await assist.GetTableSemesters();
            Assert.IsTrue(semesters.Count > 0);
            var table = await assist.GetTableCourses(semesters[0]);
            Assert.IsTrue(table.Table.Count > 0);
            Assert.IsTrue(table.Table[0].Name.Length > 0);
        }

    }
}
