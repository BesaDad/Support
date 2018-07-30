using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Books.Domain.Models
{
    public class Book
    {
        public int Id { get; set; }
        public string Tittle { get; set; }
        public int PageCount { get; set; }
        public string PublishName { get; set; }
        public int? PublishYear { get; set; }
        public string ISBN { get; set; }
        public string ImagePath { get; set; }

        public virtual List<Author> Authors { get; set; }
    }
}
