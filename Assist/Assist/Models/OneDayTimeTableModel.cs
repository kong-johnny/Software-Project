using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xiaoya.Assist.Models
{
    public class OneDayTimeTableModel
    {
        public string Name { get; set; }
        public List<TimeTableItemModel> Courses { get; set; }

        public OneDayTimeTableModel(string name)
        {
            Name = name;
            Courses = new List<TimeTableItemModel>();
        }
    }
}
