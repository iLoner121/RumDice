using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RumDice.Framework{
    /// <summary>
    /// 设置管理员
    /// </summary>
    public class SetGroupAdminSend :OneBotSend {
        public bool Enable { get; set; }
        /// <summary>
        /// 设置管理员发信包
        /// </summary>
        /// <param name="groupID">群组id</param>
        /// <param name="userID">用户id</param>
        /// <param name="enable">设置/取消管理员</param>
        public SetGroupAdminSend(long groupID,long userID,bool enable) {
            Action = OneBotAction.SetGroupAdmin;
            GroupID=groupID;
            UserID=userID;
            Enable=enable;
        }
    }
}
