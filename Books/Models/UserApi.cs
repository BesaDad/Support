using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Books.Models
{
    public class UserApi
    {
        public int ApiId { get; set; }
        public string ApiHash { get; set; }
        public string PhoneNumber { get; set; }
    }
}