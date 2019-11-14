using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tele.Infrastructure.Domain
{
    public interface IRepository<T> where T : class
    {
        IEnumerable<T> All();

        void Create(T entity);

        void Delete(T entity);

        void Update(T entity);

        IEnumerable<T> Filter(Func<T, bool> predicate);
    }
}
