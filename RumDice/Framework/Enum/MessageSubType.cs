using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RumDice.Framework{
    /// <summary>
    /// 消息的子类型
    /// </summary>
    public enum MessageSubType : int {
        /// <summary>
        ///  好友=1
        /// </summary>
        Friend = 1,
        /// <summary>
        /// 群聊消息=2
        /// </summary>
        Normal = 2,
        /// <summary>
        /// 匿名消息=3
        /// </summary>
        Anonymous = 3,
        /// <summary>
        /// 群内自身发出的消息=0
        /// </summary>
        GroupSelf = 0,
        /// <summary>
        /// 群临时会话=4
        /// </summary>
        Group = 4,
        /// <summary>
        /// 系统提示=5
        /// </summary>
        Notice = 5,
    }
}
