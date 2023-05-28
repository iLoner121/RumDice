using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RumDice.Framework {
    /// <summary>
    /// 该方法是否仅为内部自动调用的接口
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class IsOnlyInternalAttribute : Attribute {
        public IsOnlyInternalAttribute() { }
    }
}
