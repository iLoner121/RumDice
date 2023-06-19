using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RumDice.Framework {
    /// <summary>
    /// 删除单项好友
    /// </summary>
    public class DeleteNonFriendSend:QQSend {
        /// <summary>
        /// 删除单项好友发信包
        /// </summary>
        /// <param name="userID">用户id</param>
        public DeleteNonFriendSend(long userID) {
            Action = OneBotAction.DeleteNonFriend;
            UserID = userID;
        }
    }
}
