using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xiaoya.Library.User.Models
{
    public class BorrowedBook
    {
        public string Title { get; private set; }
        public string Author { get; private set; }
        public string ReturnDate { get; private set; }
        public string Fine { get; private set; }
        public string Building { get; private set; }
        public string Position { get; private set; }
        public string Description { get; private set; }

        public BorrowedBook(string title, string author, string returnDate,
            string fine, string building, string position, string description)
        {
            Title = title;
            Author = author;
            ReturnDate = returnDate;
            Fine = fine;
            Building = building;
            Position = position;
            Description = description;
        }
    }
}
