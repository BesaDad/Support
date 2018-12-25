using Books.Domain.Models;
using Books.Infrastructure.Domain;
using System.Data.Entity;
using System.Threading.Tasks;

namespace Books.BusinessLayer.Domain
{
    public class UnitOfWork : IUnitOfWork
    {
        private BookContext db;
        private Repository<Book> _book;
        private Repository<Author> _author;

        public UnitOfWork()
        {
            db = new BookContext();
        }
        public IRepository<Book> Books
        {
            get
            {
                if (_book == null)
                    _book = new Repository<Book>(db);
                return _book;
            }
        }

        public IRepository<Author> Authors
        {
            get
            {
                if (_author == null)
                    _author = new Repository<Author>(db);
                return _author;
            }
        }

        public int Save()
        {
            return db.SaveChanges();
        }

        public Task<int> SaveAsync()
        {
            return db.SaveChangesAsync();
        }
    }
}
