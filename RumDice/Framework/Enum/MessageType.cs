using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RumDice.Framework {
    /// <summary>
    ///  消息类型
    /// </summary>
    public enum MessageType : int{
        /// <summary>
        /// 私聊消息=2
        /// </summary>
        Private = 2,
        /// <summary>
        /// 群消息=1
        /// </summary>
        Group = 1,

    }
}
