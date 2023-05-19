using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RumDice.Framework {
    /// <summary>
    /// 为回复接口声明复杂关键词匹配，可以声明多条
    /// </summary>
    [AttributeUsage(AttributeTargets.Method,AllowMultiple = true)]
    public class KeyWordAttribute : Attribute {
        public string KeyWord { get; set; }
        public bool IsFullMatch { get; set; }
        public bool IsPrefix { get; set; }
        public bool IsSuffix { get; set; }
        /// <summary>
        /// 声明关键词匹配特性
        /// </summary>
        /// <param name="keyWord">多个关键词由空格隔开</param>
        /// <param name="isFullMatch">是否为全字匹配</param>
        /// <param name="isPrefix">是否为前缀</param>
        /// <param name="isSuffix">是否为后缀</param>
        public KeyWordAttribute(string keyWord, bool isFullMatch=false, bool isPrefix=false,bool isSuffix = false) {
            this.KeyWord = keyWord;
            this.IsFullMatch = isFullMatch;
            this.IsPrefix = isPrefix;
            this.IsSuffix = isSuffix;
        }

    }
}
