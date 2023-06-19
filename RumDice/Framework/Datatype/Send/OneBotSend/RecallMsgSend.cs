using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RumDice.Framework {
    /// <summary>
    /// 撤回消息
    /// </summary>
    public class RecallMsgSend : QQSend {
        public long MessageID { get; set; }
        /// <summary>
        /// 撤回消息发信包
        /// </summary>
        /// <param name="messageID">消息id（gocqhttp提供）</param>
        public RecallMsgSend(long messageID) {
            Action = OneBotAction.RecallMsg;
            MessageID = messageID;
        }
    }
}
