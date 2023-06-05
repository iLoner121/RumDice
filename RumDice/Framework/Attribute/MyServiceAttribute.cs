using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RumDice.Framework {
    /// <summary>
    /// 该方法是否为内部自动调用的接口(该接口的参数和返回值都必须为string)
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class MyServiceAttribute : Attribute {
        /// <summary>
        /// 匹配词
        /// </summary>
        public string command { get; set; }
        public MyServiceAttribute(string command) {
            this.command = command;
        }
    }
}
