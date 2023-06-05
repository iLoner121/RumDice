using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RumDice.Framework {
    /// <summary>
    /// log等级
    /// </summary>
    public enum LogLevel : int {
        Debug=1,
        Information=2,
        Warning=3,
        Error=4,
        Fatal=5,
    }
}
