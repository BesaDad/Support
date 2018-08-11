using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Books.Models
{
    public class BooksSearchViewModel
    {
        public string OrderProp { get; set; }
        public string OrderType { get; set; }
        public List<BookViewModel> Books { get; set; }
    }
}