using Tele.Domain.Models;
using Tele.Infrastructure.Business;
using Tele.Infrastructure.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tele.BusinessLayer.Services
{
    public class TeleService: ITeleService
    {
        IUnitOfWork _unitOfWork;


        public TeleService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
    }
}
