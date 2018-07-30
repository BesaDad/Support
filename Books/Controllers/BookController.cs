using AutoMapper;
using Books.Domain.Models;
using Books.Infrastructure.Business;
using Books.Infrastructure.Domain;
using Books.Models;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Books.Utility;
using System.Web;
using System.Web.Mvc;
using System.Net;

namespace Books.Controllers
{
    public class BookController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IBookService _bookService;

        public BookController(IUnitOfWork unitOfWork, IBookService bookService)
        {
            _unitOfWork = unitOfWork;
            _bookService = bookService;
        }

        public async Task<JsonResult> BookListData([DataSourceRequest] DataSourceRequest request)
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
            var result = await Mapper.Map<List<BookViewModel>>(list).ToDataSourceResultAsync(request);

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult CreateEditBook(int id = 0)
        {
            BookViewModel model = new BookViewModel();

            if (id > 0)
                model = Mapper.Map<BookViewModel>(_unitOfWork.Books.Filter(x => x.Id == id).FirstOrDefault());

            return PartialView("_CreateEditBook", model);
        }

        public ActionResult ImageSave(IEnumerable<HttpPostedFileBase> files, int bookId)
        {
            if (files != null)
            {
                foreach (var file in files)
                {
                    var book = _unitOfWork.Books.Filter(it => it.Id == bookId).FirstOrDefault();
                    var fileName = Path.GetFileName(file.FileName);
                    if (book.ImagePath != fileName || bookId == 0)
                    {
                        var physicalPath = Path.Combine(Server.MapPath("~/App_Data"), fileName);
                        file.SaveAs(physicalPath);
                    }
                }
            }
            return Content("");
        }

        public ActionResult ImageRemove(string[] fileNames)
        {
            if (fileNames != null)
            {
                foreach (var fullName in fileNames)
                {
                    var fileName = Path.GetFileName(fullName);
                    var physicalPath = Path.Combine(Server.MapPath("~/App_Data"), fileName);

                    if (System.IO.File.Exists(physicalPath))
                    {
                        System.IO.File.Delete(physicalPath);
                    }
                }
            }
            return Content("");
        }

        [HttpPost]
        public JsonResult CreateEditBook(BookViewModel model)
        {
            if (!ModelState.IsValid)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Json(new { success = false, errors = ModelState.Errors() }, JsonRequestBehavior.AllowGet);
            }

            if (!model.Authors.Any())
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                ModelState.AddModelError("", "Количество авторов должны быть не менее одного.");
                return Json(new { success = false, errors = ModelState.Errors() }, JsonRequestBehavior.AllowGet);
            }

            try
            {
                var book = Mapper.Map<Book>(model);

                if (model.Id == 0)
                    _bookService.Create(book);
                else
                    _bookService.Edit(book);

                return Json(new { success = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                ModelState.AddModelError("", "Произошла ошибка, обратитесь за помощью к администратору.");
                return Json(new { success = false, errors = ModelState.Errors(), JsonRequestBehavior.AllowGet });
            }

        }

        public JsonResult DeleteBook(int id)
        {
            try
            {
                _unitOfWork.Books.DeleteAll(it => it.Id == id);
                return Json(new { success = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                ModelState.AddModelError("", "Произошла ошибка, обратитесь за помощью к администратору.");
                return Json(new { success = false, errors = ModelState.Errors() }, JsonRequestBehavior.AllowGet);
            }

        }
    }
}