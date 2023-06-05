using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RumDice.Framework {
    /// <summary>
    /// 最基础的消息类型，所有消息都由此继承
    /// </summary>
    public class Post {
        /// <summary>
        /// 事件发生的unix时间戳
        /// </summary>
        public long Time { get; set; }
        /// <summary>
        /// 收到事件的机器人的QQ号
        /// </summary>
        public long SelfId { get; set; }
        /// <summary>
        /// 该事件的类型：消息，消息发送，请求，通知，元事件
        /// </summary>
        public PostType PostType { get; set; }

    }
}
