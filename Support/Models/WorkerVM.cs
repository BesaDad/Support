using Support.Infrastructure.Enums;
using Support.Utility;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Support.Models
{
    public class WorkerVM
    {
        public int Id { get; set; }

        [Display(Name="Имя сотрудника")]
        public string Name { get; set; }

        public int Type { get; set; }

        [Display(Name = "Тип")]
        public WorkerTypes WorkerType { get; set; }

        [Display(Name = "Должность")]
        public string TypeDescription => ((WorkerTypes) Type).GetDescription();

        [Display(Name = "Статус занятости")]
        public string Status { get; set; }
    }
}