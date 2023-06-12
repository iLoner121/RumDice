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
        /// 设置单例
        /// </summary>
        /// <param name="it">接口type</param>
        /// <param name="st">类type</param>
        /// <returns></returns>
        ValueTask SetSingleton(Type it,Type st);
        /// <summary>
        /// 设置单例
        /// </summary>
        /// <param name="st">类type</param>
        /// <returns></returns>
        ValueTask SetSingleton(Type st);
        /// <summary>
        /// 设置瞬态
        /// </summary>
        /// <param name="it">接口type</param>
        /// <param name="st">类type</param>
        /// <returns></returns>
        ValueTask SetTransient(Type it, Type st);
        /// <summary>
        /// 设置瞬态
        /// </summary>
        /// <param name="st">类type</param>
        /// <returns></returns>
        ValueTask SetTransient(Type st);
        /// <summary>
        /// 设置计时
        /// </summary>
        /// <param name="it">接口type</param>
        /// <param name="st">类type</param>
        /// <param name="time">次数</param>
        /// <returns></returns>
        ValueTask SetTimed(Type it, Type st, long time=100);
        /// <summary>
        /// 设置计时
        /// </summary>
        /// <param name="st">类type</param>
        /// <param name="time">次数</param>
        /// <returns></returns>
        ValueTask SetTimed (Type st, long time=100);
        /// <summary>
        /// 设置记数
        /// </summary>
        /// <param name="it">接口type</param>
        /// <param name="st">类type</param>
        /// <param name="max">最大数量</param>
        /// <returns></returns>
        ValueTask SetCounted(Type it, Type st, int max=10);
        /// <summary>
        /// 设置计数
        /// </summary>
        /// <param name="st">类type</param>
        /// <param name="max">最大数量</param>
        /// <returns></returns>
        ValueTask SetCounted(Type st, int max=10);

        /// <summary>
        /// 获取实例
        /// </summary>
        /// <param name="type">该类的Type</param>
        /// <returns></returns>
        object GetService(Type type);
        /// <summary>
        /// 获取多个实例
        /// </summary>
        /// <param name="type">该类的MethodInfo</param>
        /// <returns></returns>
       List<object> GetServiceList(Type type);
        /// <summary>
        /// 获取多个不同类型的实例
        /// </summary>
        /// <param name="types">每个需获取类的MethodInfo</param>
        /// <returns></returns>
        List<object> GetServiceList(List<Type> types);

        object GetStruct(string name,out Type type);
    }
}
