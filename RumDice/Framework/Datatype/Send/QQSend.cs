using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RumDice.Framework {
    public class QQSend : Send{
        public OneBotAction Action { get; set; }
        public QQSend() {
            BotType = BotType.QQbot;
            Action = OneBotAction.None;
        }
    }
}
