using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace RumDice.Framework {
    public interface IRumTimer {
        /// <summary>
        /// 启动定时器
        /// </summary>
        /// <returns></returns>
        ValueTask StartTimer();

        /// <summary>
        /// 设置定时服务（一般不用这个方法）
        /// </summary>
        /// <param name="setTimerAttribute"></param>
        void SetTimer(SetTimerAttribute setTimerAttribute);

        /// <summary>
        /// 设置定时服务（一般不用这个方法）
        /// </summary>
        /// <param name="methodInfo"></param>
        /// <param name="duration"></param>
        /// <param name="remindtime"></param>
        void SetTimer(MethodInfo methodInfo,long duration,int remindtime=-1);
        /// <summary>
        /// 设置定时服务（一般不用这个方法）
        /// </summary>
        /// <param name="methodInfo"></param>
        /// <param name="clock"></param>
        /// <param name="remindtime"></param>
        void SetTimer(MethodInfo methodInfo, string clock,int remindtime=-1);
        /// <summary>
        /// 设置定时服务
        /// </summary>
        /// <param name="action">将执行的委托</param>
        /// <param name="duration">报时间隔（秒）</param>
        /// <param name="remindtime">提醒次数（默认无限次）</param>
        void SetTimer(Action<Post> action,long duration,int remindtime=-1);
        /// <summary>
        /// 设置定时服务
        /// </summary>
        /// <param name="action">将执行的委托</param>
        /// <param name="clock">报时间隔（秒）</param>
        /// <param name="remindtime">提醒次数（默认无限次）</param>
        void SetTimer(Action<Post> action, string clock,int remindtime=-1);
        
    }
}
