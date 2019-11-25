using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Support.Infrastructure.Enums
{
    public enum ReferStates
    {
        [Description("Новый")]
        New,
        [Description("В процессе")]
        InProcess,
        [Description("Передан")]
        Transferred,
        [Description("Обработан")]
        Done,
        [Description("Отменен")]
        Canceled
    }
}
