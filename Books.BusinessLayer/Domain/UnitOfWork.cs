using Books.Domain.Models;
using Books.Infrastructure.Domain;

namespace Books.BusinessLayer.Domain
{
    public class UnitOfWork : IUnitOfWork
    {
        private IRepository<Book> _books;
        public IRepository<Book> Books => _books ?? new Repository<Book>();
    }
}
