using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RumDice.Framework {
    /// <summary>
    /// 为回复接口快速声明全字匹配，可以声明多条
    /// </summary>
    [AttributeUsage(AttributeTargets.Method,  AllowMultiple =true)]
    public class ReplyAttribute : Attribute{
        public string Reply { get; set; }
        /// <summary>
        /// 声明全字匹配特性
        /// </summary>
        /// <param name="reply">多个关键词由空格隔开</param>
        public ReplyAttribute(string reply) {
            this.Reply = reply;
        }
    }
}
