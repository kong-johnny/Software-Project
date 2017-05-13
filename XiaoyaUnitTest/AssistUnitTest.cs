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

        void Login()
        {
            var info = File.ReadLines("Assist.txt").ToList();
            assist.Username = info[0];
            assist.Password = info[1];
            var loginRes = assist.Login().Result;
            Assert.IsNull(loginRes);
        }

        [TestMethod]
        public void TestLogin()
        {
            Login();
        }

        [TestMethod]
        public void TestGetStudentInfo()
        {
            Login();
            var res = assist.GetStudentInfo().Result;
            Assert.AreEqual(res.Grade, assist.Username.Substring(0, 4));
        }

        [TestMethod]
        public void TestGetStudentDetails()
        {
            Login();
            var res = assist.GetStudentDetails().Result;
            Assert.IsTrue(res.College.Length > 0);
        }

        [TestMethod]
        public void TestGetExamScores()
        {
            Login();
            var res = assist.GetExamScores(true).Result;
            Assert.IsTrue(res.Count > 0);
            Assert.IsTrue(res[0].CourseName.Length > 0);
        }

        [TestMethod]
        public void TestGetExamArragement()
        {
            Login();
            var rounds = assist.GetExamRounds().Result;
            Assert.IsTrue(rounds.Count > 0);
            var arrangement = assist.GetExamArrangement(rounds[0]).Result;
            Assert.IsTrue(arrangement.Count > 0);
            Assert.IsTrue(arrangement[0].CourseName.Length > 0);
        }

        [TestMethod]
        public void TestGetTableCourses()
        {
            Login();
            var semesters = assist.GetTableSemesters().Result;
            Assert.IsTrue(semesters.Count > 0);
            var table = assist.GetTableCourses(semesters[0]).Result;
            Assert.IsTrue(table.Table.Count > 0);
            Assert.IsTrue(table.Table[0].Name.Length > 0);
        }

    }
}
