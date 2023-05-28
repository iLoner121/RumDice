using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RumDice.Framework {
    /// <summary>
    /// 为回复接口声明匹配优先级，默认为中位数3
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class PriorityAttribute : Attribute {
        /// <summary>
        /// 优先级
        /// </summary>
        public int Priority { get; set; }

        /// <summary>
        /// 数字越小，匹配有限度越高，默认3，范围1-5。可以突破上下限
        /// 同等级内不能保证匹配顺序
        /// </summary>
        /// <param name="priority">匹配优先级</param>
        public PriorityAttribute(int priority=3) {
            Priority = priority;
        }

    }
}
