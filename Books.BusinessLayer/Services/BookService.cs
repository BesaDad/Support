using Books.Domain.Models;
using Books.Infrastructure.Business;
using Books.Infrastructure.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Books.BusinessLayer.Services
{
    public class BookService: IBookService
    {
        IUnitOfWork _unitOfWork;


        public BookService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public void Create(Book book)
        {
            _unitOfWork.Books.Create(book);

        }

        public void Edit(Book book)
        {
           _unitOfWork.Books.Update(book);
        }
    }
}
