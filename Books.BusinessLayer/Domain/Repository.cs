using Books.Infrastructure.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Books.BusinessLayer.Domain
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private static readonly List<T> _dataSet = new List<T>();

        public List<T> All()
        {
            return _dataSet.ToList();
        }

        public void Create(T entity)
        {
            _dataSet.Add(entity);
        }

        public void Update(int index, T entity)
        {
            _dataSet.RemoveAt(index);
            _dataSet.Insert(index, entity);
        }

        public void Delete(T entity)
        {
            _dataSet.Remove(entity);
        }

        public List<T> Filter(Predicate<T> predicate)
        {
            if (predicate != null)
            {
                return _dataSet.FindAll(predicate);
            }
            else
            {
                return _dataSet;
            }
        }

        public void DeleteAll(Predicate<T> predicate)
        {
            if (predicate != null)
            {
                _dataSet.RemoveAll(predicate);
            }
        }

        public int GetIndex(Predicate<T> predicate)
        {
            return _dataSet.FindIndex(predicate);
        }
        
    }
}