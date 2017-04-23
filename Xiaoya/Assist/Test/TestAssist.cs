using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xiaoya.Assist.Test
{
    class TestAssist
    {
        public async static void Test()
        {
            Assistant assist = new Assistant("201411212027", "w1r5olQDZnf2");
            string err = await assist.Login();
            if (err != null)
                Console.WriteLine(err);
            else
                Console.WriteLine("Logined successfully.");
            var stuInfo = await assist.FetchStudentInfo();
        }
    }
}
