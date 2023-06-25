using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RumDice.Framework {
    /// <summary>
    /// 设置群名
    /// </summary>
    public class SetGroupNameSend :OneBotSend{
        public string GroupName { get; set; }
        /// <summary>
        /// 设置群名发信包
        /// </summary>
        /// <param name="groupID">群组ID</param>
        /// <param name="groupName">新群组名称</param>
        public SetGroupNameSend(long groupID,string groupName) {
            Action = OneBotAction.SetGroupName;
            GroupID = groupID;
            GroupName = groupName;
        }
    }
}
