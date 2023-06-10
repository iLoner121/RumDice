using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RumDice.Framework {
    public class FriendRecallNotice : BaseNotice {
        public long UserID { get; set; }
        /// <summary>
        /// 被撤回的消息ID
        /// </summary>
        public long MessageID { get; set; }
    }
}
