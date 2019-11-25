using Support.Domain.Models;
using Support.Infrastructure.Business;
using Support.Infrastructure.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Support.Infrastructure.Enums;

namespace Support.BusinessLayer.Services
{
    public class ReferService: IReferService
    {
        IUnitOfWork _unitOfWork;


        public ReferService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IEnumerable<Worker> GetFreeWorkers()
        {
             var bisyWorkersId = _unitOfWork.Queue.Filter(x => x.State == (int) ReferStates.InProcess)
                .Select(x => x.WorkerId).Distinct();

            return _unitOfWork.Workers.Filter(x => !bisyWorkersId.Contains(x.Id));

            //return workerForRefer.FirstOrDefault(x => x.Type == (int) WorkerTypes.Operator) ??
            //       workerForRefer.FirstOrDefault(x => x.Type == (int) WorkerTypes.Manager) ??
            //       workerForRefer.FirstOrDefault(x => x.Type == (int) WorkerTypes.Manager);
            
        }
    }
}
