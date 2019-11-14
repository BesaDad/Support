using Tele.Domain.Models;
using Tele.Infrastructure.Domain;
using System.Data.Entity;
using System.Threading.Tasks;

namespace Tele.BusinessLayer.Domain
{
    public class UnitOfWork : IUnitOfWork
    {
        private TeleContext db;
        private Repository<Paint> _paint;

        public UnitOfWork()
        {
            db = new TeleContext();
        }
   

        public IRepository<Paint> Paints
        {
            get
            {
                if (_paint == null)
                    _paint = new Repository<Paint>(db);
                return _paint;
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
