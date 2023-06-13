using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static RumDice.Framework.ServiceManager;

namespace RumDice.Framework {
    /// <summary>
    /// 在内部有回复接口的类之前注释
    /// 该特性只能在Class和Interface前面声明
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface)]
    public class MyClassAttribute : Attribute {
        public ServiceType ServiceType { get; set; }
        public MyClassAttribute(ServiceType serviceType=ServiceType.TRANSIENT) {
            this.ServiceType = serviceType;
        }
    }
}
