using AutoMapper;
using Support.Domain.Models;
using Support.Infrastructure.Business;
using Support.Infrastructure.Domain;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Support.Utility;
using System.Web;
using System.Web.Mvc;
using System.Net;
using System.Threading;
using System.Web.Helpers;
using System.Web.Management;
using TLSharp.Core;
using Support.Models;
using Support.BusinessLayer;
using Support.Infrastructure.Enums;

namespace Support.Controllers
{
    public class SupportController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IReferService _referService;

        public SupportController(IUnitOfWork unitOfWork, IReferService referService)
        {
            _unitOfWork = unitOfWork;
            _referService = referService;
        }

        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [Obsolete]
        public async Task<JsonResult> CreateRefer(ReferVM refer)
        {
            if (!ModelState.IsValid)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Json(new { success = false, errors = ModelState.Errors() }, JsonRequestBehavior.AllowGet);
            }
                
            try
            {
                var freWorkers = _referService.GetFreeWorkers().ToList();

                var newRefer = new Refer()
                {

                    ClientName = refer.ClientName,
                    Date = DateTime.Now,
                    ReferText = refer.ReferText,
                    Email = refer.Email,
                    Phone = refer.Phone
                };

                _unitOfWork.Refers.Create(newRefer);
                await _unitOfWork.SaveAsync();

                if (newRefer.Id>0)
                {
                    Worker workerForRefer = freWorkers.FirstOrDefault(x => x.Type == (int)WorkerTypes.Operator);

                    if (workerForRefer == null)
                    {
                        var timeM = int.Parse(ConfigurationSettings.AppSettings["Tm"]);
                        var timeD = int.Parse(ConfigurationSettings.AppSettings["Td"]);

                        await Task.Delay(timeM).ContinueWith((s) =>
                                workerForRefer = newRefer.Sate == (int)ReferStates.New
                                    ? freWorkers.FirstOrDefault(x => x.Type == (int)WorkerTypes.Manager)
                                    : null);

                        //Назначаем задание директору
                        if (newRefer.Sate == (int)ReferStates.New && workerForRefer == null)
                        {
                            await Task.Delay(timeD).ContinueWith((s) =>
                                    workerForRefer = newRefer.Sate == (int)ReferStates.New
                                        ? freWorkers.FirstOrDefault(x => x.Type == (int)WorkerTypes.Manager)
                                        : null);
                        }
                    }

                    if (workerForRefer != null)
                    {
                        var newQueue = new Queue()
                        {
                            WorkerId = workerForRefer.Id,
                            ReferId = newRefer.Id,
                            DateFrom = DateTime.Now,
                            State = (int) QueueStates.InProcess
                        };
                        _unitOfWork.Queue.Create(newQueue);

                        await _unitOfWork.SaveAsync();

                        return Json(new {success = true, message = "Запрос создан и передан в обработку."}, JsonRequestBehavior.AllowGet);
                    }
                    return Json(new { success = false, message = "Запрос создан и ожидает передачи в обработку." }, JsonRequestBehavior.AllowGet);
                }
                return Json(new { success = false, message = "Запрос не был создан, обратитесь за помощью к администратору" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                ModelState.AddModelError("", $"Произошла ошибка, обратитесь за помощью к администратору. {ex.Message}");
                return Json(new { success = false, errors = ModelState.Errors() }, JsonRequestBehavior.AllowGet);
            }
        }

        //public async Task<JsonResult> CancelRefer(int referId)
        //{
        //    try
        //    {
        //        _unitOfWork.
        //    }
        //    catch (Exception ex)
        //    {
        //        Response.StatusCode = (int)HttpStatusCode.BadRequest;
        //        ModelState.AddModelError("", $"Произошла ошибка, обратитесь за помощью к администратору. {ex.Message}");
        //        return Json(new { success = false, errors = ModelState.Errors() }, JsonRequestBehavior.AllowGet);
        //    }
        //}
    }
}