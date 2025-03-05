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
        public UPermissionType Permission{get; set;}
        public bool IsPrefix { get; set; }
        public bool IsSuffix { get; set; }
        public bool IsRegex { get; set; }
        public bool IsCaseSensitive { get; set; }
        public bool IsDivided { get; set; }
        /// <summary>
        /// 声明关键词匹配特性
        /// </summary>
        /// <param name="keyWord">多个关键词由空格隔开</param>
        /// <param name="isFullMatch">是否为全字匹配</param>
        /// <param name="Permission">指令权限等级，默认为Normal</param>
        /// <param name="isPrefix">是否为前缀</param>
        /// <param name="isSuffix">是否为后缀</param>
        /// <param name="isRegex">是否为正则匹配（此条为true会导致其他选项失效）</param>
        /// <param name="isCaseSensitive">是否大小写敏感</param>
        /// <param name="isDivided">该关键词在消息内是否必须被空格分隔</param>
        public KeyWordAttribute(string keyWord, bool isFullMatch=false, UPermissionType Permission=UPermissionType.Normal, bool isPrefix=false,bool isSuffix = false,bool isRegex=false, bool isCaseSensitive=false,bool isDivided=false) {
            this.KeyWord = keyWord;
            this.IsFullMatch = isFullMatch;
            this.Permission = Permission;
            this.IsPrefix = isPrefix;
            this.IsSuffix = isSuffix;
            this.IsRegex = isRegex;
            this.IsCaseSensitive = isCaseSensitive;
            this.IsDivided = isDivided;
        }

    }
}
