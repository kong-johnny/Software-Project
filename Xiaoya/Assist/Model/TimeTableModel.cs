using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xiaoya.Controls;

namespace Xiaoya.Assist.Model
{
    public class TimeTableModel
    {
        public string Name { get; set; }
        public List<TimeTableWeek> Weeks { get; set; }

        public TimeTableModel(string name)
        {
            Name = name;
            Weeks = new List<TimeTableWeek>();
        }

    }
}
