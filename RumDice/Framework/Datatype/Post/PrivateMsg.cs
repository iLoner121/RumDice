using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RumDice.Framework {
    /// <summary>
    /// 私聊对话结构
    /// </summary>
    public class PrivateMsg : BaseMsg{
        

        /// <summary>
        /// 接收者QQ号
        /// </summary>
        public long TargetID { get; set; }
        /// <summary>
        /// 临时会话来源
        /// </summary>
        public int TempSource { get; set; }
        /// <summary>
        /// 当私聊是群临时对话的话，此项不为null
        /// </summary>
        public long? GroupID { get; set; }


    }
}
