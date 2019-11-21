using Support.Domain.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Support.BusinessLayer
{
    public class SupportContext : DbContext
    {

        public SupportContext()
            : base("SupportConnectionString")
        {
            Database.SetInitializer<SupportContext>(null);
        }
        
        public DbSet<Worker> Workers { get; set; }
        public DbSet<Refer> Refers { get; set; }
        public DbSet<Queue> Queue { get; set; }

    }
}
