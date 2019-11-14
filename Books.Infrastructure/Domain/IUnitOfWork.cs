using Tele.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tele.Infrastructure.Domain
{
    public interface IUnitOfWork
    {
        IRepository<Paint> Paints { get; }
        int Save();
        Task<int> SaveAsync();
    }
}
