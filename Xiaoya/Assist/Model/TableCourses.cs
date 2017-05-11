using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xiaoya.Assist.Model
{
    public class TableCourses
    {
        /// <summary>
        /// A list of <see cref="TableCourse"/>
        /// </summary>
        [JsonProperty(PropertyName = "table")]
        public List<TableCourse> Table { get; private set; }

        /// <summary>
        /// Name of timetable
        /// </summary>
        [JsonProperty(PropertyName = "name")]
        public string Name { get; private set; }
        
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name"></param>
        /// <param name="courses"></param>
        [JsonConstructor]
        public TableCourses(string name, List<TableCourse> courses)
        {
            Table = courses;
            Name = name;
        }
    }
}
