using AutoMapper;
using Support.Domain.Models;
using Support.Infrastructure.Business;
using Support.Infrastructure.Domain;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
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
                                workerForRefer = newRefer.State == (int)ReferStates.New
                                    ? _referService.GetFreeWorkers().FirstOrDefault(x => x.Type == (int)WorkerTypes.Manager)
                                    : null);

                        //Назначаем задание директору
                        if (newRefer.State == (int)ReferStates.New && workerForRefer == null)
                        {
                            await Task.Delay(timeD).ContinueWith((s) =>
                                    workerForRefer = newRefer.State == (int)ReferStates.New
                                        ? _referService.GetFreeWorkers().FirstOrDefault(x => x.Type == (int)WorkerTypes.Director)
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

        [HttpPost]
        public async Task<JsonResult> CancelRefer(int referId)
        {
            try
            {
                var queueOnCancel =_unitOfWork.Queue
                    .Filter(x => x.ReferId == referId && x.State == (int) QueueStates.InProcess).FirstOrDefault();
                if (queueOnCancel != null)
                    queueOnCancel.State = (int) QueueStates.Canceled;

                var refer = _unitOfWork.Refers.Filter(x => x.Id == referId).FirstOrDefault();
                if (refer != null)
                    refer.State = (int) ReferStates.Canceled;
                await _unitOfWork.SaveAsync();

                return refer != null
                    ? Json(new {success = true, message = "Запрос отменен."}, JsonRequestBehavior.AllowGet)
                    : Json(new {success = false, message = "Запрос не найден."}, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                ModelState.AddModelError("", $"Произошла ошибка, обратитесь за помощью к администратору. {ex.Message}");
                return Json(new { success = false, errors = ModelState.Errors() }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public ActionResult NewRefers()
        {
            var newRefers = _unitOfWork.Refers.Filter(x => x.State == (int) ReferStates.New)?.ToList();
            var newRefersVM = new List<ReferVM>();
            if (newRefers.Any())
            {
                foreach (var newRefer in newRefers)
                {
                    newRefersVM.Add(new ReferVM()
                    {
                        Id = newRefer.Id,
                        ClientName = newRefer.ClientName,
                        Date = newRefer.Date,
                        ReferText = newRefer.ReferText,
                        Email = newRefer.Email,
                        Phone = newRefer.Phone,
                        State = newRefer.State
                    });
                }
            }

            return PartialView("_NewRefers", newRefersVM ?? new List<ReferVM>());
        }

        [HttpGet]
        public ActionResult DoneRefers()
        {
            var refersDone = _unitOfWork.Refers.Filter(x => x.State == (int)ReferStates.Done)?.ToList();
            var doneRefers = new List<ReferDone>();
            if (refersDone.Any())
            {
                foreach (var refDone in refersDone)
                {
                    var queueDone = _unitOfWork.Queue
                        .Filter(x => x.ReferId == refDone.Id && x.State == (int) ReferStates.Done)
                        .AsQueryable().Include(x=>x.Worker).FirstOrDefault();

                    doneRefers.Add(new ReferDone()
                    {
                        ReferId = refDone.Id,
                        WorkerId = queueDone.WorkerId,
                        WorkerName = queueDone.Worker.Name,
                        ClientName = refDone.ClientName,
                        ReferDate = refDone.Date,
                        SpentTime = ((TimeSpan)(queueDone.DateTo - queueDone.DateFrom)).ToString(@"hh\:mm\:ss"),
                    });
                }
            }
            
            return PartialView("_DoneRefers", doneRefers ?? new List<ReferDone>());
        }

        [HttpGet]
        public ActionResult Workers()
        {
            var workers = _unitOfWork.Workers.All();
            var workersVM = new List<WorkerVM>();
            foreach (var worker in workers)
            {
                var isBusy =
                    _unitOfWork.Queue.Filter(x => x.WorkerId == worker.Id && x.State == (int) QueueStates.InProcess).Any();
                workersVM.Add(new WorkerVM()
                {
                    Id = worker.Id,
                    Name = worker.Name,
                    Type = worker.Type,
                    Status = isBusy ? "Занят" : "Свободен"
                });
            }

            return PartialView("_Workers", workersVM);
        }

        [HttpGet]
        public ActionResult Statistic()
        {
            return View();
        }
    }
}