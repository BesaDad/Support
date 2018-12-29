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
using TeleSharp.TL.Contacts;
using TeleSharp.TL;
using TLSharp.Core;

namespace Books.Controllers
{
    public class BookController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IBookService _bookService;
        private readonly TelegramClient _client;

        private readonly int apiId = 599138;
        private readonly string apiHash = "9d6ccd0b90df866751bc615204d96839";
        private readonly string phoneNumber = "2348073897000";

        public BookController(IUnitOfWork unitOfWork, IBookService bookService)
        {
            _unitOfWork = unitOfWork;
            _bookService = bookService;
        }

        public ActionResult BookListData(string orderProp, string orderType)
        {
            TempData["OrderProp"] = orderProp;
            TempData["OrderType"] = orderType;

            var list = _unitOfWork.Books.All();

            var model = Mapper.Map<List<BookViewModel>>(list.ToList().GetSortetList(orderProp, orderType));

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

        [HttpGet]
        public async Task<ActionResult> CheckStart()
        {
            CheckParamsViewModel checkParams = new CheckParamsViewModel();
            checkParams.Hash = "";


            return PartialView("_CheckList", checkParams);
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

                _unitOfWork.Save();
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
                var books = _unitOfWork.Books.Filter(it => it.Id == id);
                if (books.Any())
                {
                    foreach (var book in books)
                    {
                        _unitOfWork.Books.Delete(book);
                    }
                }

                return Json(new { success = true, message = "Книга успешно удалена." }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                ModelState.AddModelError("", "Произошла ошибка, обратитесь за помощью к администратору.");
                return Json(new { success = false, errors = ModelState.Errors() }, JsonRequestBehavior.AllowGet);
            }

        }

        [HttpPost]
        public async Task<JsonResult> TeleCheck(string code, string hash)
        {
            try
            {
                var client = new TLSharp.Core.TelegramClient(apiId, apiHash);
                await client.ConnectAsync();

                var user = new TLUser();
                if (!client.IsUserAuthorized())
                {
                    var hash1 = await client.SendCodeRequestAsync(phoneNumber);
                    var code1 = "65904";
                    user = await client.MakeAuthAsync(phoneNumber, hash1, code1);
                }

                if (client.IsConnected)
                {
                    try
                    {
                        TLRequestGetContacts requestImportContacts = new TLRequestGetContacts();
                        var contacts = new List<TLAbsUser>();
                        var o2 = await client.GetContactsAsync();
                        var users = o2.Users.ToList();
                        foreach (var us in users)
                        {
                            var newUs = (TLUser)us;
                            if (!_unitOfWork.Paints.Filter(it => it.Phone == newUs.Phone).Any())
                            {
                                _unitOfWork.Paints.Create(new Paint
                                {
                                    Phone = newUs.Phone,
                                    Username = newUs.Username,
                                    AccessHash = newUs.AccessHash,
                                    AccountId = newUs.Id,
                                    FirstName = newUs.FirstName,
                                    LastName = newUs.LastName,
                                    LangCode = newUs.LangCode,
                                    Verified = newUs.Verified,
                                    BotChatHistory = newUs.BotChatHistory,
                                    Bot = newUs.Bot,
                                    Deleted = newUs.Deleted,
                                    MutualContact = newUs.MutualContact,
                                    Contact = newUs.Contact,
                                    Self = newUs.Self,
                                    Flags = newUs.Flags,
                                    BotInlinePlaceholder = newUs.BotInlinePlaceholder
                                });
                            }
                        }

                        _unitOfWork.Save();

                        return Json(new {success = true, message = "Список проверен."}, JsonRequestBehavior.AllowGet);

                    }

                    catch (Exception ex)
                    {
                        Response.StatusCode = (int) HttpStatusCode.BadRequest;
                        ModelState.AddModelError("", "Произошла ошибка, обратитесь за помощью к администратору.");
                        return Json(new {success = false, errors = ModelState.Errors()}, JsonRequestBehavior.AllowGet);
                    }
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Соединение разорвано." }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { success = false, message = "Соединение разорвано." }, JsonRequestBehavior.AllowGet);
        }
    }
}