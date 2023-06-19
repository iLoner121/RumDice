using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RumDice.Framework {
    /// <summary>
    /// 取消禁言
    /// </summary>
    public class CancelBanGroupMemberSend :QQSend{
        /// <summary>
        /// 取消禁言发信包
        /// </summary>
        /// <param name="groupID">群组id</param>
        /// <param name="userID">用户id</param>
        public CancelBanGroupMemberSend(long groupID,long userID) {
            Action = OneBotAction.CancelBanGroupMember;
            GroupID = groupID;
            UserID = userID;
        }
    }
}
