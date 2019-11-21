using Support.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Support.Infrastructure.Domain
{
    public interface IUnitOfWork
    {
        IRepository<Worker> Workers { get; }
        IRepository<Refer> Refers { get; }
        IRepository<Queue> Queue { get; }
        int Save();
        Task<int> SaveAsync();
    }
}
