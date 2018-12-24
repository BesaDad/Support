using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Books.Domain.Models
{
    [Table("Books")]
    public class Book
    {
        [Column("id")]
        public int Id { get; set; }

        public string Tittle { get; set; }

        [Column("page_count")]
        public int PageCount { get; set; }

        [Column("publish_name")]
        public string PublishName { get; set; }

        [Column("publish_year")]
        public int? PublishYear { get; set; }

        [Column("isbn")]
        public string ISBN { get; set; }

        [Column("image_path")]
        public string ImagePath { get; set; }

        public virtual ICollection<Author> Authors { get; set; }
    }
}
