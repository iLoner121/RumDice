using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RumDice.Framework {
    /// <summary>
    /// 是否为私聊接口
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class IsPrivateAttribute : Attribute {
        /// <summary>
        /// 是否仅为私聊接口
        /// </summary>
        public bool IsOnlyPrivate { get; set; }

        public IsPrivateAttribute() {
            IsOnlyPrivate = false;
        }

        public IsPrivateAttribute(bool isOnlyPrivate=false) { 
            IsOnlyPrivate = isOnlyPrivate;
        }
    }
}
