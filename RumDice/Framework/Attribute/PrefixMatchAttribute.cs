using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RumDice.Framework {
    /// <summary>
    /// 自动前缀指令的匹配。等于自动添加句点和句号开头的/大小写不敏感的前缀匹配；以及没有句点和句号开头的/必须被空格分隔出来的/大小写不敏感的前缀匹配
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class PrefixMatchAttribute : Attribute {
        public string Prefix { get; set; }

        /// <summary>
        /// 声明前缀指令
        /// </summary>
        /// <param name="prefix">前缀词本身（系统会自动添加句点和句号）</param>
        public PrefixMatchAttribute(string prefix) {
            this.Prefix = prefix;
        }
    }
}
