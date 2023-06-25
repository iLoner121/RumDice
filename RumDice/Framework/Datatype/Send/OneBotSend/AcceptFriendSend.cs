using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RumDice.Framework {
    /// <summary>
    /// 通过好友申请
    /// </summary>
    public class AcceptFriendSend:OneBotSend {
        public String Flag { get; set; }
        public String? Remark { get; set; }
        /// <summary>
        /// 通过好友申请发信包
        /// </summary>
        /// <param name="flag">好友申请标识码（在好友请求包中获得）</param>
        /// <param name="remark">备注（可以为空）</param>
        public AcceptFriendSend(String flag,string? remark=null) {
            Action = OneBotAction.AcceptFriend;
            Flag = flag;
            Remark = remark;
        }
    }
}
