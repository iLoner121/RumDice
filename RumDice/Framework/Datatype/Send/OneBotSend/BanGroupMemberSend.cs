using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RumDice.Framework {
    /// <summary>
    /// 禁言群员
    /// </summary>
    public class BanGroupMemberSend : OneBotSend{
        public TimeSpan Duration { get; set; }
        /// <summary>
        /// 禁言群员发信包
        /// </summary>
        /// <param name="groupID">群组id</param>
        /// <param name="userID">用户id</param>
        /// <param name="duration">禁言时间</param>
        public BanGroupMemberSend(long groupID,long userID,TimeSpan duration) {
            Action = OneBotAction.BanGroupMember;
            GroupID = groupID;
            UserID = userID;
            Duration = duration;
        }
    }
}
