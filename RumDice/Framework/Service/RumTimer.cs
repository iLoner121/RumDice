using RumDice.Core;
using RumDice.Module;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace RumDice.Framework {
    public class RumTimer : IRumTimer {
        static System.Threading.Timer _timer;
        readonly IServiceManager _serviceManager;
        readonly IRumLogger _logger;
        readonly IEventManager _eventManager;

        List<SetTimerAttribute> Timers = new();

        void HandleTimers(object obj) {
            List<SetTimerAttribute> remove = new();
            lock (Timers) {
                DateTime now = DateTime.Now;
                foreach (var timer in Timers) {
                    if (timer.IsClock) {
                        DateTime clock = Convert.ToDateTime(timer.ClockTime);
                        if (!(now.Hour == clock.Hour && now.Minute == clock.Minute && now.Second == clock.Second))
                            continue;
                        if (timer.RemindTime != -1) {
                            if (--timer.RemindTime == 0)
                                remove.Add(timer);
                        }
                        if (timer.IsMethodInfo) {
                            InvokeMethod(timer.MethodInfo);
                        } else {
                            InvokeMethod(timer.Action);
                        }
                        continue;
                    }
                    long tsNow = DateTimeOffset.Now.ToUnixTimeSeconds();
                    if (tsNow - timer.LastTimeStamp < timer.Duration)
                        continue;

                    if (timer.RemindTime != -1) {
                        if (--timer.RemindTime == 0)
                            remove.Add(timer);
                    }
                    timer.LastTimeStamp = tsNow;
                    if (timer.IsMethodInfo) {
                        InvokeMethod(timer.MethodInfo);
                    } else {
                        InvokeMethod(timer.Action);
                    }
                    continue;
                }
                foreach(var item in remove) {
                    Timers.Remove(item);
                }
            }
        }
        void InvokeMethod(MethodInfo method) {
            var service = _serviceManager.GetService(method.DeclaringType);
            if(service==null) 
                return;
            method.Invoke(service, new object[]{null});

        }
        void InvokeMethod(Action<Post> action) {
            action(null);
        }

        public RumTimer(IServiceManager serviceManager, IRumLogger logger, IEventManager eventManager) {
            _serviceManager = serviceManager;
            _logger = logger;
            _eventManager = eventManager;
        }

        public void SetTimer(SetTimerAttribute setTimerAttribute) {
            Timers.Add(setTimerAttribute);
        }

        public void SetTimer(MethodInfo methodInfo, long duration,int times=-1) {
            var t = new SetTimerAttribute(duration,times);
            t.MethodInfo = methodInfo;
            Timers.Add(t);
        }

        public void SetTimer(MethodInfo methodInfo, string clock,int times=-1) {
            var t = new SetTimerAttribute(clock,times);
            t.MethodInfo= methodInfo;
            Timers.Add(t);
        }

        public void SetTimer(Action<Post> action, long duration, int times = -1) {
            var t = new SetTimerAttribute(action,duration,times);
            Timers.Add(t);
        }

        public void SetTimer(Action<Post> action, string clock,int times =-1) {
            var t = new SetTimerAttribute(action,clock,times);
            Timers.Add(t);
        }

        public async ValueTask StartTimer() {
            await Task.Delay(0);
            var assemblys = Assembly.GetAssembly(typeof(IExample)).ExportedTypes
                .ToList();
            foreach (var assembly in assemblys) {
                if (assembly.GetCustomAttribute(typeof(MyClassAttribute)) is not MyClassAttribute)
                    continue;
                // 获取内部方法
                var methods = assembly.GetMethods();
                foreach (var method in methods) {
                    foreach (var att in method.GetCustomAttributes()) {
                        if (att is not SetTimerAttribute a)
                            continue;
                        try {
                            if (a.IsMethodInfo) {
                                a.MethodInfo = method;
                            }
                            SetTimer(a);
                            _logger.Debug("CoreData", "$$$导入定时服务成功");
                        }
                        catch (Exception ex) {
                            _logger.Error(ex, "$$$导入定时服务失败");
                        }
                    }
                }
            }

            _timer = new System.Threading.Timer(HandleTimers, null, 10000, 1000);
        }
    }
}
