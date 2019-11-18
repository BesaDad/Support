using Tele.Domain.Models;
using Tele.Infrastructure.Domain;
using System.Data.Entity;
using System.Threading.Tasks;

namespace Tele.BusinessLayer.Domain
{
    public class UnitOfWork : IUnitOfWork
    {
        private TeleContext db;
        private Repository<Contact> _contacts;

        public UnitOfWork()
        {
            db = new TeleContext();
        }
   

        public IRepository<Contact> Contacts
        {
            get
            {
                if (_contacts == null)
                    _contacts = new Repository<Contact>(db);
                return _contacts;
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
