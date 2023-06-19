using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RumDice.Framework {
    /// <summary>
    /// 退群
    /// </summary>
    public class LeaveGroupSend:QQSend {
        /// <summary>
        /// 退群发信包
        /// </summary>
        /// <param name="groupID">群组id</param>
        public LeaveGroupSend(long groupID) {
            Action = OneBotAction.LeaveGroup;
            GroupID = groupID;
        }
    }
}
