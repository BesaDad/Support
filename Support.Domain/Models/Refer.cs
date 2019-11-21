using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace Support.Domain.Models
{
    [Table("Refers")]
    public class Refer
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("client_name")]
        public string ClientName { get; set; }

        [Column("date")]
        public DateTime Date { get; set; }

        [Column("state")]
        public int Sate { get; set; }

        [Column("refer_text")]
        public string ReferText { get; set; }

        [Column("email")]
        public string Email { get; set; }

        [Column("phone")]
        public int Phone { get; set; }

        public ICollection<Queue> Queue { get; set; }
    }
}
