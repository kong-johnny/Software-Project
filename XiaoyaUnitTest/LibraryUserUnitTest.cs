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

        async Task Login()
        {
            var info = File.ReadLines("Library.txt").ToList();
            client.Username = info[0];
            client.Password = info[1];
            var res = await client.Login();
            Assert.IsNull(res);
        }

        [TestMethod]
        public async Task TestLogin()
        {
            await Login();
        }

        [TestMethod]
        public async Task TestGetBorrowedBooks()
        {
            await Login();
            var list = await client.GetBorrowedBooks();
            Assert.IsNotNull(list);
        }

        [TestMethod]
        public async Task TestRenewAll()
        {
            await Login();
            var renewRes = await client.RenewAll();
            Assert.IsTrue(renewRes.Contains("续借"));
        }
    }
}
