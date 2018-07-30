using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Books.Infrastructure.Domain
{
    public interface IRepository<T> where T : class
    {
        List<T> All();

        void Create(T entity);

        void Update(int index, T entity);

        int GetIndex(Predicate<T> predicate);

        void Delete(T entity);

        List<T> Filter(Predicate<T> predicate);

        void DeleteAll(Predicate<T> predicate);
    }
}
