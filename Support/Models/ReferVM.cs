using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Support.Models
{
    public class ReferVM
    {
        public int Id { get; set; }

        [Display(Name = "Имя клиента")]
        public string ClientName { get; set; }

        [Display(Name = "Текст обращения")]
        public string ReferText { get; set; }

        [Display(Name = "Эл. почта")]
        public string Email { get; set; }

        [Display(Name = "Телефон")]
        public string Phone { get; set; }

        [Display(Name = "Статус обращения")]
        public int State { get; set; }

        [Display(Name = "Дата поступления")]
        public DateTime Date { get; set; }
    }
}