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

        public ActionResult CreateRefer()
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
                //Создаем новое обращение
                var newRefer = new Refer()
                {

                    ClientName = refer.ClientName,
                    Date = DateTime.Now,
                    ReferText = refer.ReferText,
                    Email = refer.Email,
                    Phone = refer.Phone
                };

                newRefer = _unitOfWork.Refers.Create(newRefer);

                await _unitOfWork.SaveAsync();

                if (newRefer.Id>0)
                {
                    //Находим свободных операторов
                    Worker workerForRefer = _referService.GetFreeWorkers().FirstOrDefault(x => x.Type == (int)WorkerTypes.Operator);

                    //Если нет свободных операторов
                    if (workerForRefer == null)
                    {
                        var timeM = int.Parse(ConfigurationSettings.AppSettings["Tm"]);
                        var timeD = int.Parse(ConfigurationSettings.AppSettings["Td"]);

                        //Если нет свободных операторов, то ждем их, иначе передаем свободному менеджеру
                        await Task.Delay(timeM).ContinueWith((s) =>
                                workerForRefer = newRefer.State == (int)ReferStates.New
                                    ? _referService.GetFreeWorkers()
                                        .Where(x => x.Type == (int)WorkerTypes.Manager || x.Type == (int)WorkerTypes.Operator).OrderBy(x=>x.Type)
                                        .FirstOrDefault()
                                    : null);

                        //Если нет свободных менеджеров, то ждем их, иначе передаем свободному директору
                        if (newRefer.State == (int)ReferStates.New && workerForRefer == null)
                        {
                            await Task.Delay(timeD).ContinueWith((s) =>
                                    workerForRefer = newRefer.State == (int)ReferStates.New
                                        ? _referService.GetFreeWorkers().OrderBy(x => x.Type)
                                            .FirstOrDefault()
                                        : null);
                        }
                    }

                    //Если свободный сотрудник в итоге обнаружился, то создаем событие.
                    if (workerForRefer != null)
                    {
                        var newQueue = new Queue()
                        {
                            WorkerId = workerForRefer.Id,
                            ReferId = newRefer.Id,
                            DateFrom = DateTime.Now,
                            State = (int) ReferStates.InProcess
                        };
                        _unitOfWork.Queue.Create(newQueue);
                        newRefer.State = (int) ReferStates.InProcess;

                        await _unitOfWork.SaveAsync();

                        return Json(new {success = true, message = "Запрос создан и передан в обработку."}, JsonRequestBehavior.AllowGet);
                    }
                    return Json(new { success = true, message = "Запрос создан и ожидает передачи в обработку." }, JsonRequestBehavior.AllowGet);
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
        public async Task<JsonResult> ChangeReferState(int referId, int state, int queueId)
        {
            //Если нет запрос не на завершение или отмену обращения, то возврашаем неудачное завершение.
            if(queueId != (int)ReferStates.Done && queueId != (int)ReferStates.Canceled)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Json(new { success = false, message = "Некорректный статус." }, JsonRequestBehavior.AllowGet);
            }

            try
            {
                var stateText = "";
                var refer = _unitOfWork.Refers.FindById(referId);
                var queue = _unitOfWork.Queue.FindById(queueId);

                if (state == (int)ReferStates.Done)
                {
                    queue.State = state;
                    refer.State = (int)ReferStates.Done;
                    stateText = ReferStates.Done.GetDescription();                
                }

                queue.State = state;
                refer.State = (int)ReferStates.Canceled;
                stateText = ReferStates.Canceled.GetDescription();

                //После отмены/завершения назначаем освободившемуся сотруднику новое обращение, если такое есть.
                var newRefer = _unitOfWork.Refers.Filter(x=>x.State == (int)ReferStates.New).OrderBy(x => x.Date).FirstOrDefault();
                if (newRefer != null)
                {
                    var newQueue = new Queue()
                    {
                        WorkerId = queue.WorkerId,
                        ReferId = newRefer.Id,
                        DateFrom = DateTime.Now,
                        State = (int)ReferStates.InProcess
                    };
                    _unitOfWork.Queue.Create(newQueue);
                }

                await _unitOfWork.SaveAsync();
                return Json(new { success = true, message = $"Статус запроса изменен на \"{stateText}\"." }, JsonRequestBehavior.AllowGet);           
            }
            catch (Exception ex)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                ModelState.AddModelError("", $"Произошла ошибка, обратитесь за помощью к администратору. {ex.Message}");
                return Json(new { success = false, errors = ModelState.Errors() }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public async Task<JsonResult> TransferRefer(int referId, int queueId, int workerId)
        {
            //Если запрос не на передачу обращения другому сотруднику, то возврашаем неудачное завершение.
            if (queueId != (int)ReferStates.Transferred)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Json(new { success = false, message = "Некорректный статус." }, JsonRequestBehavior.AllowGet);
            }

            try
            {
                var queue = _unitOfWork.Queue.FindById(queueId);
                queue.State = (int)ReferStates.Transferred;
                var worker = _unitOfWork.Workers.FindById(workerId);


                var newQueue = new Queue()
                {
                    WorkerId = workerId,
                    ReferId = referId,
                    DateFrom = DateTime.Now,
                    State = (int)ReferStates.InProcess
                };
                _unitOfWork.Queue.Create(newQueue);

                //После передачи назначаем освободившемуся сотруднику новое обращение, если такое есть.
                var newRefer = _unitOfWork.Refers.Filter(x => x.State == (int)ReferStates.New).OrderBy(x => x.Date).FirstOrDefault();
                if (newRefer != null)
                {
                    var newQueueForCurWorker = new Queue()
                    {
                        WorkerId = queue.WorkerId,
                        ReferId = newRefer.Id,
                        DateFrom = DateTime.Now,
                        State = (int)ReferStates.InProcess
                    };
                    _unitOfWork.Queue.Create(newQueueForCurWorker);
                }

                await _unitOfWork.SaveAsync();
                return Json(new { success = true, message = $"Запрос передан сотруднику \"{worker.Name}\"." }, JsonRequestBehavior.AllowGet);
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
                    .Filter(x => x.ReferId == referId && x.State == (int)ReferStates.InProcess).FirstOrDefault();
                if (queueOnCancel != null)
                    queueOnCancel.State = (int)ReferStates.Canceled;

                var refer = _unitOfWork.Refers.FindById(referId);
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
            var newRefers = _unitOfWork.Refers.Filter(x => x.State == (int) ReferStates.New)?.OrderBy(x=>x.Date).ToList();
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

            return PartialView("_NewRefers");
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
            
            return PartialView("_DoneRefers");
        }

        [HttpGet]
        public ActionResult Workers()
        {
            var workers = _unitOfWork.Workers.All();
            var workersVM = new List<WorkerVM>();
            foreach (var worker in workers)
            {
                var isBusy =
                    _unitOfWork.Queue.Filter(x => x.WorkerId == worker.Id && x.State == (int)ReferStates.InProcess).Any();
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
        public ActionResult CreateEditWorker(int id = 0)
        {
            WorkerVM model = new WorkerVM();
            var worker = _unitOfWork.Workers.FindById(id);
            if (id > 0)
            {
                model.Id = worker.Id;
                model.Name = worker.Name;
                model.Type = worker.Type;
                model.WorkerType = (WorkerTypes) worker.Type;
            }

            return PartialView("_CreateEditWorker", model);
        }

        [HttpPost]
        public async Task<JsonResult> CreateEditWorker(WorkerVM worker)
        {
            try
            {
                if (worker.Id > 0)
                {
                    var curWorker = _unitOfWork.Workers.FindById(worker.Id);
                    curWorker.Name = worker.Name;
                    curWorker.Type = (int)worker.WorkerType;
                }
                else
                {
                    var newWroker = new Worker()
                    {
                        Name = worker.Name,
                        Type = (int)worker.WorkerType
                    };
                    _unitOfWork.Workers.Create(newWroker);
                }

                await _unitOfWork.SaveAsync();

                return Json(new { success = true, message = "Данные сотрудника изменены." }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                ModelState.AddModelError("", $"Произошла ошибка, обратитесь за помощью к администратору. {ex.Message}");
                return Json(new { success = false, errors = ModelState.Errors() }, JsonRequestBehavior.AllowGet);
            }
        }


        [HttpGet]
        public ActionResult Statistic()
        {
            return View();
        }
    }
}