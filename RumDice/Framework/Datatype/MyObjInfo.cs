using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RumDice.Framework.Datatype {
    /// <summary>
    /// 已解析的数据对象的信息
    /// </summary>
    public class MyObjInfo {
        /// <summary>
        /// json文件名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 解析后的文件对象
        /// </summary>
        public object Obj { get; set; }
        /// <summary>
        /// 使用次数
        /// </summary>
        public int UsedTime { get; set; } = 0;
        /// <summary>
        /// 最大使用次数
        /// </summary>
        public int MaxTime { get; set; } = 10;
        /// <summary>
        /// 是否为插件
        /// </summary>
        public bool IsPlugin { get; set; }=false;
    }
}
