using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Tele.Models
{
    public class ReferVM
    {
        public int Id { get; set; }
        public string ClientName { get; set; }
        public string ReferText { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
    }
}