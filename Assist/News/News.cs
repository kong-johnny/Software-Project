using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xiaoya.News
{
    public class News
    {
        public string Title { get; private set; }
        public string Date { get; private set; }
        public Uri Url { get; private set; }

        public News(string title, string date, Uri url)
        {
            Title = title;
            Date = date;
            Url = url;
        }
    }
}
