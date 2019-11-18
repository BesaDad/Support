using AutoMapper;
using Tele.Domain.Models;
using Tele.Infrastructure.Business;
using Tele.Infrastructure.Domain;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Tele.Utility;
using System.Web;
using System.Web.Mvc;
using System.Net;
using System.Web.Helpers;
using System.Web.Management;
using TeleSharp.TL.Contacts;
using TeleSharp.TL;
using TLSharp.Core;
using Books.Models;

namespace Tele.Controllers
{
    public class TeleController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ITeleService _teleService;
        private readonly TelegramClient _client;

        //private readonly int apiId = 773874;
        //private readonly string apiHash = "8a71d40e68df57548df4433b0eb7c1e3";
        //private readonly string phoneNumber = "77470914908";

        public TeleController(IUnitOfWork unitOfWork, ITeleService teleService)
        {
            _unitOfWork = unitOfWork;
            _teleService = teleService;
        }

        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<JsonResult> ContactsList(UserApi userApi)
        {
            try
            {
                var t = _unitOfWork.Contacts.All();
                if (!ModelState.IsValid)
                {
                    Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    return Json(new { success = false, errors = ModelState.Errors() }, JsonRequestBehavior.AllowGet);
                }

                var client = new TLSharp.Core.TelegramClient(userApi.ApiId, userApi.ApiHash);
                await client.ConnectAsync();

                var user = new TLUser();
                if (!client.IsUserAuthorized())
                {
                    var hash = await client.SendCodeRequestAsync(userApi.PhoneNumber.ToString());
                    var checkCode = "65904";//значение из смс
                    user = await client.MakeAuthAsync(userApi.PhoneNumber.ToString(), hash, checkCode);
                }

                if (client.IsConnected)
                {
                    try
                    {
                        TLRequestGetContacts requestImportContacts = new TLRequestGetContacts();
                        var contacts = new List<TLAbsUser>();
                        var o2 = await client.GetContactsAsync();
                        var allUsers = _unitOfWork.Contacts.All().ToList();
                        var users = o2.Users.ToList();
                        foreach (var us in users)
                        {
                            var newUs = (TLUser)us;
                            if (!allUsers.Where(it => it.Phone == newUs.Phone).Any())
                            {
                                _unitOfWork.Contacts.Create(new Contact
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
                                    IsContact = newUs.Contact,
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
                        ModelState.AddModelError("", $"Произошла ошибка, обратитесь за помощью к администратору. {ex.Message}");
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