using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Books.Models
{
    public class UserApi
    {
        [Required]
        [DisplayName("Id пользователя")]
        public int ApiId { get; set; }

        [Required]
        [DisplayName("Хеш-код пользователя")]
        public string ApiHash { get; set; }

        [Required]
        [DisplayName("Номер телефона")]
        public int PhoneNumber { get; set; }
    }
}