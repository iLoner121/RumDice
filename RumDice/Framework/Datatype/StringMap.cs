using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RumDice.Framework {
    /// <summary>
    /// 一种标准的储存格式（你当然可以自己定义各种各样的格式）
    /// </summary>
    [MyStruct]
    public class StringMap {
        /// <summary>
        /// 存储信息的表
        /// </summary>
        public Dictionary<string, string> table { get; set; } = new();
        /// <summary>
        /// 备注
        /// </summary>
        public string remark { get; set; } = null;
        public StringMap(string remark = null) {
            this.remark = remark;
        }
    }
}
