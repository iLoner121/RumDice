using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RumDice.Framework {
    /// <summary>
    /// 提出群组成员
    /// </summary>
    public class KickGroupMemberSend : QQSend {
        /// <summary>
        /// 是否拒绝申请
        /// </summary>
        public bool RejectRequest { get; set; }
        /// <summary>
        /// 提出群组成员发信包
        /// </summary>
        /// <param name="groupID">群组id</param>
        /// <param name="userID">成员id</param>
        /// <param name="rejectRequest">是否拒绝申请</param>
        public KickGroupMemberSend(long groupID,long userID,bool rejectRequest) {
            Action = OneBotAction.KickGroupMember;
            GroupID = groupID; 
            UserID = userID;
            RejectRequest = rejectRequest;
        }
    }
}
