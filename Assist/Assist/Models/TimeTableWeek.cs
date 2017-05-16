using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xiaoya.Assist.Models
{
    public class TimeTableWeek
    {
        public string Name { get; set; }
        public List<TimeTableItemModel> Items { get; set; }

        public TimeTableWeek(string name)
        {
            Name = name;
            Items = new List<TimeTableItemModel>();
        }
    }
}
