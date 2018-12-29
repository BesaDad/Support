using Books.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Books.Infrastructure.Domain
{
    public interface IUnitOfWork
    {
        IRepository<Book> Books { get; }
        IRepository<Author> Authors { get; }
        IRepository<Paint> Paints { get; }
        int Save();
        Task<int> SaveAsync();
    }
}
