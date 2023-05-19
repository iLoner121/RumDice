using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RumDice.Framework {
    /// <summary>
    /// 当该结构体会被序列化存储时需要添加该注释
    /// 该特性不会被派生类继承
    /// 该特性只能在Class和Struct上使用
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
    public class MyStructAttribute : Attribute {

    }
}
