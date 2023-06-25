using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace RumDice.Framework {
    [AttributeUsage(AttributeTargets.Method)]
    public class SetTimerAttribute :Attribute{
        public bool IsClock { get; set; }
        public bool IsMethodInfo { get; set; }
        public int RemindTime { get; set; }
        public MethodInfo? MethodInfo { get; set; }
        public Action<Post>? Action { get; set; }
        public long? LastTimeStamp { get; set; }
        public long? Duration { get; set; }
        public string? ClockTime { get; set; }
        /// <summary>
        /// 设置定时器（系统将以MyClass中定义的方法生成对象并调用）
        /// </summary>
        /// <param name="duration">定时间隔（秒）</param>
        /// <param name="times">作用次数（默认无限次）</param>
        public SetTimerAttribute(long duration, int times = -1) {
            if(times<=0)
                times = -1;
            IsClock = false;
            IsMethodInfo = true;
            Duration = duration;
            LastTimeStamp = DateTimeOffset.Now.ToUnixTimeSeconds();
            RemindTime = times;
        }
        /// <summary>
        /// 设置定时器（系统将以MyClass中定义的方法生成对象并调用）
        /// </summary>
        /// <param name="clock">报时时间（时分秒）</param>
        /// <param name="times">作用次数（默认无限次）</param>
        public SetTimerAttribute(string clock,int times = -1) {
            if (times <= 0)
                times = -1;
            IsClock = true;
            IsMethodInfo = true;
            ClockTime = clock;
            RemindTime = times;
        }

        /// <summary>
        /// 一般用不上
        /// </summary>
        /// <param name="action">计时器将执行的委托</param>
        /// <param name="duration"></param>
        /// <param name="times"></param>
        public SetTimerAttribute(Action<Post> action,long duration, int times = -1) {
            if (times <= 0)
                times = -1;
            IsClock = false;
            IsMethodInfo = false;
            Duration = duration;
            LastTimeStamp = DateTimeOffset.Now.ToUnixTimeSeconds();
            Action = action;
            RemindTime = times;
        }

        /// <summary>
        /// 一般用不上
        /// </summary>
        /// <param name="action">计时器将执行的委托</param>
        /// <param name="clock"></param>
        /// <param name="times"></param>
        public SetTimerAttribute(Action<Post> action,string clock,int times = -1) {
            if (times <= 0)
                times = -1;
            IsClock = true;
            IsMethodInfo = false;
            ClockTime = clock;
            RemindTime = times;
            Action = action;
        }
    }
}
