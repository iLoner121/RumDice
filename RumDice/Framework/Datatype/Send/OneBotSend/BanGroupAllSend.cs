using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RumDice.Framework {
    /// <summary>
    /// 全体禁言
    /// </summary>
    public class BanGroupAllSend : OneBotSend {
        /// <summary>
        /// 全体禁言发信包
        /// </summary>
        /// <param name="groupID">群号</param>
        public BanGroupAllSend(long groupID) {
            Action = OneBotAction.BanGroupAll;
            GroupID = groupID;
        }
    }
}

