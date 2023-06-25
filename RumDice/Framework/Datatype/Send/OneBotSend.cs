using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RumDice.Framework {
    public class OneBotSend : Send{
        public OneBotAction Action { get; set; }
        public OneBotSend() {
            BotType = BotType.QQbot;
            Action = OneBotAction.None;
        }
    }
}
