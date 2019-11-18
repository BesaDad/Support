using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tele.Domain.Models
{
    [Table("Queue")]
    public class Queue
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("refer_id")]
        public int ReferId { get; set; }

        [Column("worker_id")]
        public int WorkerId { get; set; }

        [Column("date_from")]
        public DateTime DateFrom { get; set; }

        [Column("date_to")]
        public DateTime DateTo { get; set; }

        [Column("state")]
        public int State { get; set; }

        [ForeignKey("WorkerId")]
        public Worker  Worker { get; set; }

        [ForeignKey("ReferId")]
        public Refer Refer { get; set; }
    }
}
