using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RumDice.Framework {
    /// <summary>
    /// 添加事件监听
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class ListenAttribute: Attribute {
        public AllType Type { get; set; }
        /// <summary>
        /// 添加监听
        /// </summary>
        /// <param name="type">事件类型</param>
        public ListenAttribute(AllType type) {
            Type = type;
        }
    }
}
