using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Support.Models
{
    public class ReferDone
    {
        public int WorkerId { get; set; }
        public int ReferId { get; set; }

        [Display(Name = "Имя сотрудника")]
        public string WorkerName { get; set; }

        [Display(Name = "Имя клиента")]
        public string ClientName { get; set; }

        [Display(Name = "Дата поступления")]
        public DateTime ReferDate { get; set; }

        [Display(Name = "Время выполнения")]
        public string SpentTime { get; set; }
    }
}