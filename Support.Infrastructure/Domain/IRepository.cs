using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Support.Infrastructure.Domain
{
    public interface IRepository<T> where T : class
    {
        IEnumerable<T> All();

        T Create(T entity);

        void Delete(T entity);

        void Update(T entity);

        IEnumerable<T> Filter(Func<T, bool> predicate);
    }
}
