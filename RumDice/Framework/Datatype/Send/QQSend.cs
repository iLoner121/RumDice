using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RumDice.Framework {
    public class QQSend : Send,ICloneable{
        public QQSend() {
            BotType = BotType.QQbot;
        }
    }
}
