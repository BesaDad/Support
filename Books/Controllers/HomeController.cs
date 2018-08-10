using AutoMapper;
using Books.Domain.Models;
using Books.Infrastructure.Domain;
using Books.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Books.Controllers
{
    public class HomeController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public HomeController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public ActionResult Index()
        {
            if (!_unitOfWork.Books.All().Any())
            {
                var initialBooks = new List<Book> {
                    new Book
                    {
                        Id = 1,
                        Tittle = "Робинзон Крузо",
                        PageCount = 300,
                        PublishName = "Лунный свет",
                        PublishYear = 2011,
                        Authors = new List<Author>
                        {
                            new Author {
                                Id = 1,
                                BookId = 1,
                                LastName = "Стивенсон",
                                FirstName = "Роберт"
                            }
                        }
                    },
                    new Book
                    {
                        Id = 2,
                        Tittle = "Братья Карамазовы",
                        PageCount = 500,
                        PublishName = "Старая Россия",
                        PublishYear = 2003,
                        Authors = new List<Author>
                        {
                            new Author {
                                Id = 2,
                                BookId = 2,
                                LastName = "Федор",
                                FirstName = "Достоевский"
                            }
                        }
                    }
                };

                foreach (var b in initialBooks)
                {
                    _unitOfWork.Books.Create(b);
                }
            }

            var list = _unitOfWork.Books.All();
            var model = Mapper.Map<List<BookViewModel>>(list);

            return View(model);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {

            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}