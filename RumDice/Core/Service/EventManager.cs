using RumDice.Framework;
using RumDice.Module;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Intrinsics.Arm;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace RumDice.Core {
    public class EventManager : IEventManager {
        readonly IGlobalData _globalData;
        readonly IServiceManager _serviceManager;

        public EventManager(IGlobalData globalData,
            IServiceManager serviceManager) {
            _globalData = globalData;
            _serviceManager = serviceManager;
        }

        public async ValueTask HandleGroupMessage(string s) {
            /*
             * 在信息匹配环节，消息包内的消息本身会进过如下的流程
             * 以此判断是否匹配：
             * 内置简单匹配
             * 插件简单匹配
             * 内置复杂匹配
             * 插件复杂匹配
             * 一旦在任意环节匹配到，后续的环节将不会被访问
             * 因此此顺序也是指令的优先级
             */

            bool isMatch = false;
            MethodInfo method = null;
            if (!isMatch) {
                // inner reply
            }
            if (!isMatch) {
                // plugin reply
            }
            if(!isMatch) {
                // inner match
                var innerFuncs = _globalData.InnerMatchTable;
                foreach (var innerFunc in innerFuncs) {
                    if (MatchKeyWord(s, innerFunc.Key)) {
                        isMatch = true;
                        method = innerFunc.Value;
                        break;
                    }
                }
            }
            if (!isMatch) {
                // plugin match
            }

            if (isMatch) {
                Console.WriteLine("已匹配到关键词");
                var service = _serviceManager.GetService(method.DeclaringType);
                if (service != null) {
                    method.Invoke(service, new object[] { s });
                } else {
                    Console.WriteLine("获取类型失败");
                }
            } else {
                Console.WriteLine("未匹配到关键词");
            }
        }


        /// <summary>
        /// 复杂匹配
        /// </summary>
        /// <param name="message">用户的输入字符串</param>
        /// <param name="attributes">KeyWordAttribute列表</param>
        /// <returns></returns>
        bool MatchKeyWord(string message,List<KeyWordAttribute> attributes) {
            bool c1 = true, c2 = true, c3 = true, c4 = true;
            foreach (KeyWordAttribute attribute in attributes) {
                string[] keywords = attribute.KeyWord.Split(' ');
                c1 = true; c2 = true; c3 = true; c4=true;
                foreach(string keyword in keywords) {
                    // 是否符合全匹配
                    if (attribute.IsFullMatch&&c2) {
                        if(message.Equals(keyword)) {
                            c2 = false;
                        }
                    }
                    // 是否符合前缀
                    if(attribute.IsPrefix&&c3) {
                        if(message.StartsWith(keyword)) {
                            c3= false;
                        }
                    }
                    // 是否符合后缀
                    if (attribute.IsSuffix && c4) {
                        if(message.EndsWith(keyword)) {
                            c4 = false;
                        }
                    }
                    // 是否符合模糊匹配
                    if (c1) {
                        if (message.Contains(keyword)){
                            c1 = false;
                        }
                    }

                }
                if (c1 || (attribute.IsFullMatch && c2) || (attribute.IsPrefix && c3) || (attribute.IsSuffix && c4)) {
                    return false;
                }
            }
            return true;
        }
    }
}
