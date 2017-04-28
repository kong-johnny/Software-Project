using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xiaoya.Assist.Test
{
    class TestAssist
    {
        public async static Task<string> Test()
        {
            string text = "";
            Assistant assist = new Assistant("201411212027", "w1r5olQDZnf2");
            string err = await assist.Login();
            if (err != null)
                text += err;
            var stuInfo = await assist.FetchStudentInfo();
            var details = await assist.GetStudentDetails();

            var examRounds = await assist.GetExamRounds();
            foreach(var round in examRounds)
            {
                text += round.Code + ": " + round.Name + "\n";
            }

            var examScores = await assist.GetExamScores(true);
            foreach(var score in examScores)
            {
                text += score.Term + "  " + score.CourseName + ": " + score.Score + "\n";
            }

            return text;
         }
    }
}
