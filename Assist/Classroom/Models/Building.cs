using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xiaoya.Classroom.Models
{
    public class Building
    {
        public string Id { get; set; }
        public string Name { get; set; }

        public Building(string id, string name)
        {
            if (Id !=  null) { Id = id.Trim(); }
            else { Id = "0"; }
            if (Name != null) { Name = name.Trim(); }
            else { Name = "jiaojiu"; }
        }
    }
}
