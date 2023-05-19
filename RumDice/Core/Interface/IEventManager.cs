using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RumDice.Core {
    public interface IEventManager {

        ValueTask HandleGroupMessage(string k);
    }
}
