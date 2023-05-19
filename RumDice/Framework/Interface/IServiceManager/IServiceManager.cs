using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace RumDice.Framework {
    public interface IServiceManager {
        ValueTask Initialize();

        /// <summary>
        /// 将该类设置为单例
        /// </summary>
        /// <param name="methodInfo">目标类的MethodInfo</param>
        /// <returns></returns>
        ValueTask SetSingleton(Type it,Type st);
        /// <summary>
        /// 将该类设置为常新（每次获取总是新的示例）
        /// </summary>
        /// <param name="methodInfo">目标类的MethodInfo</param>
        /// <returns></returns>
        ValueTask SetTransient(Type it, Type st);
        /// <summary>
        /// 将该类设为定时存在（在一定时间内会获取相同的示例）
        /// </summary>
        /// <param name="methodInfo">目标类的MethodInfo</param>
        /// <param name="time">实例的生命周期（timestamp）</param>
        /// <returns></returns>
        ValueTask SetTimed(Type it, Type st, long time=100);
        /// <summary>
        /// 将该类设定为定数示例（在示例总数未超过上限时，总是获取新示例）
        /// </summary>
        /// <param name="methodInfo">目标类的MethodInfo</param>
        /// <param name="max">实例的数量上限</param>
        /// <returns></returns>
        ValueTask SetCounted(Type it, Type st, int max=10);
        /// <summary>
        /// 获取实例
        /// </summary>
        /// <param name="type">该类的Type</param>
        /// <returns></returns>
        object GetService(Type type);
        /// <summary>
        /// 获取多个实例
        /// </summary>
        /// <param name="methodInfo">该类的MethodInfo</param>
        /// <returns></returns>
       List<object> GetServiceList(Type type);
        /// <summary>
        /// 获取多个不同类型的实例
        /// </summary>
        /// <param name="methodInfo">每个需获取类的MethodInfo</param>
        /// <returns></returns>
        List<object> GetServiceList(List<Type> types);
    }
}
