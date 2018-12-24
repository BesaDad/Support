using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Books.Domain.Models
{
    [Table("Authors")]
    public class Author
    {
        [Column("id")]
        public int Id { get; set; }

        [Column("book_id")]
        public int BookId { get; set; }

        [Column("first_name")]
        public string FirstName { get; set; }

        [Column("last_name")]
        public string LastName { get; set; }

        [ForeignKey("BookId")]
        public Book Book { get; set; }
    }
}
