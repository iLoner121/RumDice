using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RumDice.Framework {
    /// <summary>
    /// 作为存储模板的泛型表（不能直接使用）
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class AnyMap<T> {
        /// <summary>
        /// 存储信息的表
        /// </summary>
        /// </summary>
        public Dictionary<string, T> table { get; set; } = new();
        /// <summary>
        /// 备注
        /// </summary>
        public string remark { get; set; } = null;
        public AnyMap(string remark = null) {
            this.remark = remark;
        }
    }
}
