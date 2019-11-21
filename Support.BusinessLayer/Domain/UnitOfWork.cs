using Support.Domain.Models;
using Support.Infrastructure.Domain;
using System.Data.Entity;
using System.Threading.Tasks;

namespace Support.BusinessLayer.Domain
{
    public class UnitOfWork : IUnitOfWork
    {
        private SupportContext db;
        private Repository<Worker> _workers;
        private Repository<Refer> _refers;
        private Repository<Queue> _queue;

        public UnitOfWork()
        {
            db = new SupportContext();
        }
   

        public IRepository<Worker> Workers
        {
            get
            {
                if (_workers == null)
                    _workers = new Repository<Worker>(db);
                return _workers;
            }
        }

        public IRepository<Refer> Refers
        {
            get
            {
                if (_refers == null)
                    _refers = new Repository<Refer>(db);
                return _refers;
            }
        }

        public IRepository<Queue> Queue
        {
            get
            {
                if (_queue == null)
                    _queue = new Repository<Queue>(db);
                return _queue;
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
