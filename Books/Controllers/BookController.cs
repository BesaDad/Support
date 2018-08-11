using AutoMapper;
using Books.Domain.Models;
using Books.Infrastructure.Business;
using Books.Infrastructure.Domain;
using Books.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Books.Utility;
using System.Web;
using System.Web.Mvc;
using System.Net;
using System.Web.Helpers;
using System.Web.Management;

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

        public ActionResult BookListData()
        {
            var list = _unitOfWork.Books.All();
            var model = Mapper.Map<List<BookViewModel>>(list);

            return PartialView("_BookList", model);
        }

        [HttpGet]
        public ActionResult CreateEditBook(int id = 0)
        {
            BookViewModel model = new BookViewModel();

            if (id > 0)
                model = Mapper.Map<BookViewModel>(_unitOfWork.Books.Filter(x => x.Id == id).FirstOrDefault());

            return PartialView("_CreateEditBook", model);
        }

        [HttpPost]
        public JsonResult ImageSave(HttpPostedFileBase image)
        {
            try
            {
                if (image != null)
                {
                    var fileName = Path.GetFileName(image.FileName);
                    var physicalPath = Path.Combine(Server.MapPath("~/App_Data"), fileName);
                    string tempfileName = "";

                    if (System.IO.File.Exists(physicalPath))
                    {
                        int counter = 2;
                        while (System.IO.File.Exists(physicalPath))
                        {
                            tempfileName = $"{Path.GetFileNameWithoutExtension(image.FileName)}_{counter}{Path.GetExtension(image.FileName)}";
                            physicalPath = Path.Combine(Server.MapPath("~/App_Data"), tempfileName);
                            counter++;
                        }

                        fileName = tempfileName;
                    }
                    
                    image.SaveAs(physicalPath);
                    return Json(new {success = true, name = fileName, message = "Изображение загружено"}, JsonRequestBehavior.AllowGet);
                }

                return Json(new { success = false, message = "Выберите изборажение" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Изображение не загружено" }, JsonRequestBehavior.AllowGet);
            }

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
                string messageSuccess;
                if (model.Id == 0)
                {
                    _bookService.Create(book);
                    messageSuccess = "Книга успешно добавлена.";
                }
                else
                {
                    _bookService.Edit(book);
                    messageSuccess = "Книга успешно изменена.";
                }

                return Json(new { success = true, message = messageSuccess }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                ModelState.AddModelError("", "Произошла ошибка, обратитесь за помощью к администратору.");
                return Json(new { success = false, errors = ModelState.Errors(), JsonRequestBehavior.AllowGet });
            }

        }

        public ActionResult GetAuthorRow(int id, int bookId, int authCount)
        {
            var book = new BookViewModel();
            for(int i = 0; i <= authCount; i ++)
            {
                book.Authors.Add(new AuthorViewModel {Id = id + 1, BookId = bookId});
            };
            return PartialView("_AuthorRow", book);
        }

        public JsonResult DeleteBook(int id)
        {
            try
            {
                _unitOfWork.Books.DeleteAll(it => it.Id == id);
                return Json(new { success = true, message = "Книга успешно удалена." }, JsonRequestBehavior.AllowGet);
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