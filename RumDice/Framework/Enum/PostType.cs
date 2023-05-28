using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RumDice.Framework {
    public enum PostType : int {
        /// <summary>
        /// 消息=1
        /// </summary>
        Message = 1,
        /// <summary>
        /// 机器人自己发出的消息=0
        /// </summary>
        MessageSent =0,  
        // 请求=2
        Request = 2 ,
        // 通知=3
        Notice = 3,
        // 元时间=4
        MetaEvent = 4,
    }
}
