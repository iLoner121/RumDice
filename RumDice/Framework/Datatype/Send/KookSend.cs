using Kook;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace RumDice.Framework {
    public class KookSend : Send{
        public ICard? Card { get; set; }
        public KookMsgType KookMsgType { get; set; }
        public KookSend(KookMsgType kookMsgType = KookMsgType.Code) {
            KookMsgType = kookMsgType;
            BotType = BotType.KOOKbot;
        }
    }
}
