using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RumDice.Framework;

namespace RumDice.Core {
    public interface IEventManager {

        ValueTask HandleGroupMessage(Post post);

    }
}
