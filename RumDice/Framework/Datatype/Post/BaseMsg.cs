using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RumDice.Framework {
    /// <summary>
    /// 消息类型的数据包的基础格式。所有Message均包含此数据包内容
    /// </summary>
    public class BaseMsg : Post {
        /// <summary>
        /// 消息类型
        /// </summary>
        public MsgType MsgType { get; set; }
        /// <summary>
        /// 消息子类型
        /// </summary>
        public MsgSubType MsgSubType { get; set; }
        /// <summary>
        /// 消息ID
        /// </summary>
        public long MsgID { get; set; }
        /// <summary>
        /// 发件人ID
        /// </summary>
        public long UserID { get; set; }
        /// <summary>
        /// 因为kook它不是用id而是用用户名+四位标识码的，所以我加了个这个参数
        /// </summary>
        public string? Username { get; set; }
        /// <summary>
        /// 纯文字消息String
        /// </summary>
        public string Msg { get; set; }
        /// <summary>
        /// CQ码格式string
        /// </summary>
        public string RawMsg { get; set; }
        /// <summary>
        /// 字体
        /// </summary>
        public int? Font { get; set; }
        /// <summary>
        /// 发信人昵称
        /// </summary>
        public string NickName { get; set; }
        /// <summary>
        /// 发信人性别
        /// </summary>
        public string? Sex { get; set; }
        /// <summary>
        /// 发信人年龄
        /// </summary>
        public int? Age {get; set; }


    }
}
