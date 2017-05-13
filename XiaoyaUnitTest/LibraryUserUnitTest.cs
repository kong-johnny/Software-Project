using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xiaoya.Library.User;

namespace XiaoyaUnitTest
{
    [TestClass]
    public class LibraryUserUnitTest
    {
        LibraryClient client = new LibraryClient();

        void Login()
        {
            var info = File.ReadLines("Library.txt").ToList();
            client.Username = info[0];
            client.Password = info[1];
            var res = client.Login().Result;
            Assert.IsNull(res);
        }

        [TestMethod]
        public void TestLogin()
        {
            Login();
        }

        [TestMethod]
        public void TestGetBorrowedBooks()
        {
            Login();
            var list = client.GetBorrowedBooks().Result;
            Assert.IsNotNull(list);
        }

        [TestMethod]
        public void TestRenewAll()
        {
            Login();
            var renewRes = client.RenewAll().Result;
            Assert.IsTrue(renewRes.Contains("续借"));
        }
    }
}
