using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RumDice.Framework {
    /// <summary>
    /// 设置专属头衔
    /// </summary>
    public class SetGroupTitleSend:OneBotSend {
        public String Title { get; set; }
        /// <summary>
        /// 设置专属头衔发信包
        /// </summary>
        /// <param name="groupID">群组id</param>
        /// <param name="userID">用户id</param>
        /// <param name="title">专属头衔</param>
        public SetGroupTitleSend(long groupID,long userID,String title) {
            Action = OneBotAction.SetGroupTitle;
            GroupID = groupID;
            UserID = userID;
            Title = title;
        }
    }
}
