using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace Books.Models
{
    public class UserApi
    {
        [DisplayName("Id пользователя")]
        public int ApiId { get; set; }

        [DisplayName("Хеш пользователя")]
        public string ApiHash { get; set; }

        [DisplayName("Номер телефона")]
        public string PhoneNumber { get; set; }
    }
}