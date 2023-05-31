using EleCho.GoCqHttpSdk.Action;
using RumDice.Framework;
using RumDice.Module;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
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

        public async ValueTask HandlePrivateMessage(Post post) {
            var baseMsg = (BaseMessage)post;
            string s = baseMsg.Msg;
            /*
             * 在信息匹配环节，消息包内的消息本身会进过如下的流程
             * 以此判断是否匹配：
             * 内置简单匹配
             * 插件简单匹配
             * 复杂匹配
             * 一旦在任意环节匹配到，后续的环节将不会被访问
             * 因此此顺序也是指令的优先级
             */

            bool isMatch = false;
            MethodInfo method = null;
            int minPriority = _globalData.MinPriority;
            int maxPriority = _globalData.MaxPriority;
            if (!isMatch) {
                // inner reply
            }
            if (!isMatch) {
                // plugin reply
            }
            if (!isMatch) {
                // Match
                // 遍历优先级
                for (int i = minPriority; i <= maxPriority; i++) {
                    // 提取对应优先级
                    var innerTempDic = _globalData.MatchTable
                        .Where(z => _globalData.FuncTable[z.Value].Priority == i)
                        .ToDictionary(z => z.Key, z => z.Value);
                    if (innerTempDic.Count == 0)
                        continue;
                    foreach (var temp in innerTempDic) {
                        // 该接口是否可以用于群聊
                        if (_globalData.FuncTable[temp.Value].Scope == 1)
                            continue;
                        // 是否匹配
                        if (!MatchKeyWord(s, temp.Key))
                            continue;

                        isMatch = true;
                        method = _globalData.FuncTable[temp.Value].MethodInfo;
                        break;
                    }
                    if (isMatch)
                        break;
                }
            }

            if (isMatch) {
                Console.WriteLine("已匹配到关键词");
                Invoke(method, post);
            } else {
                Console.WriteLine("未匹配到关键词");
            }

        }

        public async ValueTask HandleGroupMessage(Post post) {
            var baseMsg = (BaseMessage)post;
            string s = baseMsg.Msg;
            /*
             * 在信息匹配环节，消息包内的消息本身会进过如下的流程
             * 以此判断是否匹配：
             * 内置简单匹配
             * 插件简单匹配
             * 复杂匹配
             * 一旦在任意环节匹配到，后续的环节将不会被访问
             * 因此此顺序也是指令的优先级
             */

            bool isMatch = false;
            MethodInfo method = null;
            int minPriority = _globalData.MinPriority;
            int maxPriority = _globalData.MaxPriority;
            if (!isMatch) {
                // inner reply
            }
            if (!isMatch) {
                // plugin reply
            }
            if(!isMatch) {
                // Match
                // 遍历优先级
                for(int i = minPriority; i <= maxPriority; i++) {
                    // 提取对应优先级
                    var innerTempDic = _globalData.MatchTable
                        .Where(z => _globalData.FuncTable[z.Value].Priority== i)
                        .ToDictionary(z=>z.Key,z=>z.Value);
                    if (innerTempDic.Count == 0)
                        continue;
                    foreach(var temp in innerTempDic) {
                        // 该接口是否可以用于群聊
                        if (_globalData.FuncTable[temp.Value].Scope == 3)
                            continue;
                        // 是否匹配
                        if (!MatchKeyWord(s, temp.Key))
                            continue;
                       
                        isMatch = true;
                        method = _globalData.FuncTable[temp.Value].MethodInfo;
                        break;
                    }
                    if (isMatch)
                        break;
                }
            }

            if (isMatch) {
                Console.WriteLine("已匹配到关键词");
                Invoke(method, post);
            } else {
                Console.WriteLine("未匹配到关键词");
            }
        }


        async ValueTask Invoke(MethodInfo method, Post post) {
            var service = _serviceManager.GetService(method.DeclaringType);
            if (service == null) {
                Console.WriteLine("获取类型失败");
                return;
            }
            
            var res = method.Invoke(service, new object[] { post });
            if (res == null) {
                Console.WriteLine("未获得返回值");
                return;
            } 
            Console.WriteLine(res.GetType().FullName);
            if(res is string s) {
                await SendMessage(s);
                return;
            }
            if(res is Send send) {
                await SendMessage(send);
                return;
            }
            if(res is List<Send> sends) {
                await SendMessage(sends);
                return;
            }
            Console.WriteLine("错误的返回类型");
            return;
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
                    // 是否符合模糊匹配
                    if (c1) {
                        if (message.Contains(keyword)) {
                            c1 = false;
                        }
                    }
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
                }
                if (c1 || (attribute.IsFullMatch && c2) || (attribute.IsPrefix && c3) || (attribute.IsSuffix && c4)) {
                    return false;
                }
            }
            return true;
        }

        async ValueTask SendMessage(string s) {
            Console.WriteLine(s);

        }
        async ValueTask SendMessage(Send send) {
            Console.WriteLine(send.ToString());
        }
        async ValueTask SendMessage(List<Send> sends) {
            Console.WriteLine("List<Send>");
        }   
    }
}
