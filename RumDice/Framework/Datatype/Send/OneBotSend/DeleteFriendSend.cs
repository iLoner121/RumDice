using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RumDice.Framework {
    /// <summary>
    /// 删除好友
    /// </summary>
    public class DeleteFriendSend:OneBotSend {
        /// <summary>
        /// 删除好友发信包
        /// </summary>
        /// <param name="userID">用户id</param>
        public DeleteFriendSend(long userID) {
            Action = OneBotAction.DeleteFriend;
            UserID = userID;
        }
    }
}
