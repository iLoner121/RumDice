using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RumDice.Framework {
    /// <summary>
    /// 文件信息
    /// </summary>
    public class MyFileInfo {
        /// <summary>
        /// 文件名
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 文件地址
        /// </summary>
        public string Location { get; set; }
        /// <summary>
        /// 读取类型：3永远在内存里 2使用十次后销毁 1每次重新读取
        /// </summary>
        public int ReadType { get; set; }
        /// <summary>
        /// 对象类型：该json将被解析为的类型
        /// </summary>
        public Type ObjType { get; set; }
        /// <summary>
        /// 是否为插件来源
        /// </summary>
        public bool IsPlugin { get; set; }=false;
    }
}
