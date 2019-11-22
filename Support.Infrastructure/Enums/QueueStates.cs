using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Support.Infrastructure.Enums
{
    public enum QueueStates
    {
        [Description("Выполняется")]
        InProcess,
        [Description("Выполнен")]
        Done,
        [Description("Передан")]
        Transferred,
        [Description("Отменен")]
        Canceled
    }
}
