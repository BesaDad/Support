using Books.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Books.Infrastructure.Business
{
    public interface IBookService
    {
        void Create(Book book);
        void Edit(Book book);
    }
}
