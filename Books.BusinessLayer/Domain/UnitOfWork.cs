using Books.Domain.Models;
using Books.Infrastructure.Domain;
using System.Data.Entity;
using System.Threading.Tasks;

namespace Books.BusinessLayer.Domain
{
    public class UnitOfWork : DbContext, IUnitOfWork
    {
        private static DbContext _context;

        public UnitOfWork() 
            : base("TeleConnectionString")
        {
            _context.Configuration.LazyLoadingEnabled = false;
        }

        private IRepository<Book> _books;
        public IRepository<Book> Books => _books ?? new Repository<Book>();

        public int Save()
        {
            return _context.SaveChanges();
        }

        public Task<int> SaveAsync()
        {
            return _context.SaveChangesAsync();
        }
    }
}
