﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RumDice.Framework {
    public class CancelBanGroupAllSend: OneBotSend {
        public CancelBanGroupAllSend(long groupID) {
            Action=OneBotAction.CancelBanGroupAll;
            GroupID=groupID;
        }
    }
}
