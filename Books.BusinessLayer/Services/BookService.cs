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
            foreach (var author in book.Authors.Select((value, index) => new { Value = value, Index = index }))
            {
                author.Value.Id = author.Index + 1;
                author.Value.BookId = book.Id;
            }

            var lastBook = _unitOfWork.Books.All().OrderByDescending(it => it.Id).First();
            book.Id = lastBook.Id + 1;
            _unitOfWork.Books.Create(book);
        }

        public void Edit(Book book)
        {
            var lastAuthorId = book.Authors.OrderByDescending(it => it.Id).FirstOrDefault().Id;
            foreach (var author in book.Authors.Where(it=>it.Id == 0))
            {
                author.Id = ++lastAuthorId;
                author.BookId = book.Id;
            }

            var indexByUpdate = _unitOfWork.Books.GetIndex(it => it.Id == book.Id);
            _unitOfWork.Books.Update(indexByUpdate, book);
        }
    }
}
