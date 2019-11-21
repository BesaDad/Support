using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Support.Models
{
    public class ReferDone
    {
        public int WorkerId { get; set; }
        public int ReferId { get; set; }
        public string WorkerName { get; set; }
        public string ClientName { get; set; }
        public DateTime ReferDate { get; set; }
        public string SpentTime { get; set; }
    }
}