using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace RumDice.Framework {
    /// <summary>
    /// 存储用户自定义接口的信息
    /// </summary>
    public class MyMethodInfo {
        /// <summary>
        /// 对应的MethodInfo
        /// </summary>
        public MethodInfo MethodInfo { get; set; }
        /// <summary>
        /// 匹配优先级：默认3
        /// </summary>
        public int Priority { get; set; }
        /// <summary>
        /// 作用域 1:仅群聊 2:群聊与私聊都可以使用 3:仅私聊
        /// </summary>
        public int Scope { get; set; }

        /// <summary>
        /// 是否是插件(默认为否)
        /// </summary>
        public bool IsPlugin { get; set; } = false;

        public bool IsOnlyInternal { get; set; } = false;

        public MyMethodInfo(MethodInfo methodInfo) {
            MethodInfo = methodInfo;
        }

    }
}
