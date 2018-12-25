using Books.Domain.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Books.BusinessLayer
{
    public class BookContext : DbContext
    {

        public BookContext()
            : base("TeleConnectionString")
        {
        }
        
        public DbSet<Book> Books { get; set; }
        public DbSet<Author> Authors { get; set; }


    }
}
