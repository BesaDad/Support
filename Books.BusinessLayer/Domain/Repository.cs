using Books.Infrastructure.Domain;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Books.BusinessLayer.Domain
{
    public class Repository<T> : IRepository<T> where T : class
    {

        private static readonly DbContext _context;
        private static readonly DbSet<T> _dbSet;

        //public Repository(DbContext context)
        //{
        //    _context = context;
        //    _dbSet = context.Set<T>();
        //}

        public IEnumerable<T> All()
        {
            return _dbSet.ToList();
        }

        public void Create(T entity)
        {
            _dbSet.Add(entity);
        }

        public void Update(T entity)
        {
            _context.Entry(entity).State = EntityState.Modified;
        }

        public void Delete(T entity)
        {
            _dbSet.Remove(entity);
        }

        public IEnumerable<T> Filter(Func<T, bool> predicate)
        {
            return _dbSet.AsNoTracking().Where(predicate).ToList();
        }

        public int Save(T entity)
        {
             return _context.SaveChanges();
        }
    }
}