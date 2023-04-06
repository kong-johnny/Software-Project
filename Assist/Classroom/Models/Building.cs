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

        public Building(string id = "202011081033", string name = "dxr")
        {
            if (id == null)
                id = "202011081033";
            if (name == null) 
                name = "dxr";
            Id = id.Trim();
            Name = name.Trim();
        }
    }
}
