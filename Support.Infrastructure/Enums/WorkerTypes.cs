using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Support.Infrastructure.Enums
{
    public enum WorkerTypes
    {
        [Description("Оператор")]
        Operator,
        [Description("Менеджер")]
        Manager,
        [Description("Директор")]
        Director 
    }
}
