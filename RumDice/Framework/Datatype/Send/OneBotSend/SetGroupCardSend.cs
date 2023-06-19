using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RumDice.Framework {
    /// <summary>
    /// 设置群名片
    /// </summary>
    public class SetGroupCardSend :QQSend{
        public String CardName { get; set; }
        /// <summary>
        /// 设置群名片发信包
        /// </summary>
        /// <param name="groupID">群组ID</param>
        /// <param name="userID">用户ID</param>
        /// <param name="cardName">群名片</param>
        public SetGroupCardSend(long groupID,long userID,string cardName) {
            Action = OneBotAction.SetGroupCard;
            GroupID = groupID;
            UserID = userID;
            CardName = cardName;
        }
    }
}
