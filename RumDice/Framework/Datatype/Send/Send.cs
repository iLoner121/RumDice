using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RumDice.Framework {
    /// <summary>
    /// 回复消息类型
    /// </summary>
    public class Send :ICloneable {
        /// <summary>
        /// 回复类型为群聊/私聊（可不填写）
        /// </summary>
        public MsgType MsgType { get; set; } = MsgType.None;
        /// <summary>
        /// 回复消息内容
        /// </summary>
        public string Msg { get; set; } = string.Empty;
        /// <summary>
        /// 私聊目标ID（如为私聊回复需要填写）
        /// </summary>
        public long UserID { get; set; } = 0;
        /// <summary>
        /// 群聊目标ID（如为群聊回复需要填写）
        /// </summary>
        public long GroupID { get; set; } = 0;
        /// <summary>
        /// 是否将 Msg 解析为 CQ 码
        /// </summary>
        public bool IsCQ { get; set; }=false;

        /// <summary>
        /// 机器人类型
        /// </summary>
        public BotType BotType { get; set; }

        public object Clone() {
            return this.MemberwiseClone();
        }
    }
}
