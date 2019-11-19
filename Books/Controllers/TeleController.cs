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
using Tele.Models;
using Tele.Infrastructure.Enums;

namespace Tele.Controllers
{
    public class TeleController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IReferService _referService;
        private readonly TelegramClient _client;

        //private readonly int apiId = 773874;
        //private readonly string apiHash = "8a71d40e68df57548df4433b0eb7c1e3";
        //private readonly string phoneNumber = "77470914908";

        public TeleController(IUnitOfWork unitOfWork, IReferService referService)
        {
            _unitOfWork = unitOfWork;
            _referService = referService;
        }

        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<JsonResult> CreateRefer(ReferVM refer)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    return Json(new { success = false, errors = ModelState.Errors() }, JsonRequestBehavior.AllowGet);
                }

                
                try
                {
                    var freWorkers = _referService.GetFreeWorkers();
                    var newReferModel = new Refer()
                    {
                        ClientName = refer.ClientName,
                        Date = DateTime.Now,
                        ReferText = refer.ReferText,
                        Email = refer.Email,
                        Phone = refer.Phone
                    };

                    var newRefer = _unitOfWork.Refers.Create(newReferModel);

                    var workerForRefer = freWorkers.FirstOrDefault(x => x.Type == (int) WorkerTypes.Operator) ??
                                         freWorkers.FirstOrDefault(x => x.Type == (int) WorkerTypes.Manager) ??
                                         freWorkers.FirstOrDefault(x => x.Type == (int) WorkerTypes.Manager);

                    var newQueue = new Queue()
                    {
                        WorkerId = workerForRefer.Id,
                        ReferId = newRefer.Id,
                        DateFrom = DateTime.Now,
                        State = (int)QueueStates.InProcess
                    };

                    _unitOfWork.Queue.Create(newQueue);

                    await _unitOfWork.SaveAsync();

                    return Json(new { success = true, message = "Запрос создан и передан в обработку." }, JsonRequestBehavior.AllowGet);

                }

                catch (Exception ex)
                {
                    Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    ModelState.AddModelError("", $"Произошла ошибка, обратитесь за помощью к администратору. {ex.Message}");
                    return Json(new { success = false, errors = ModelState.Errors() }, JsonRequestBehavior.AllowGet);
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