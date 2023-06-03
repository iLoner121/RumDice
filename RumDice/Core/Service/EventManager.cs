using EleCho.GoCqHttpSdk.Action;
using Microsoft.Extensions.DependencyInjection;
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
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace RumDice.Core {
    public class EventManager : IEventManager {
        readonly ICoreData _globalData;
        readonly IServiceManager _serviceManager;
        readonly IClientConnector _clientConnector;

        readonly int _minPriority;
        readonly int _maxPriority;


        public EventManager(ICoreData globalData,
            IServiceManager serviceManager,
            IClientConnector clientConnector) {
            _globalData = globalData;
            _serviceManager = serviceManager;
            _clientConnector = clientConnector;

            _minPriority=globalData.MinPriority;
            _maxPriority=globalData.MaxPriority;
        }

        public async void HandlePrivateMessage(Post post) {
            var baseMsg = (BaseMessage)post;
            MethodInfo method;
            bool isMatch = MatchFunc(baseMsg.Msg, 1, out method);

            if (isMatch) {
                Console.WriteLine("已匹配到关键词");
                Invoke(method, post);
            } else {
                Console.WriteLine("未匹配到关键词");
            }

        }

        public async void HandleGroupMessage(Post post) {
            var baseMsg = (BaseMessage)post;
            MethodInfo method;
            bool isMatch = MatchFunc(baseMsg.Msg,3, out method);

            if (isMatch) {
                Console.WriteLine("已匹配到关键词");
                Invoke(method, post);
            } else {
                Console.WriteLine("未匹配到关键词");
            }
        }

        /// <summary>
        /// 调用匹配到的服务
        /// </summary>
        /// <param name="method"></param>
        /// <param name="post"></param>
        /// <returns></returns>
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
                await SendMessage(post,s);
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
        /// 匹配全过程
        /// </summary>
        /// <param name="scopeCheck">不符合的scope</param>
        /// <param name="method">返回的method</param>
        /// <returns></returns>
        public bool MatchFunc(string msg,int scopeCheck,out MethodInfo method) {
            method = null;
            // TODO:匹配reply

            // 匹配复杂方法
            for (int i = _minPriority; i <= _maxPriority; i++) {
                // 提取对应优先级
                var innerTempDic = _globalData.MatchTable
                    .Where(z => _globalData.FuncTable[z.Value].Priority == i)
                    .ToDictionary(z => z.Key, z => z.Value);
                if (innerTempDic.Count == 0)
                    continue;
                foreach (var temp in innerTempDic) {
                    // 该接口是否可以用于群聊
                    if (_globalData.FuncTable[temp.Value].Scope == scopeCheck)
                        continue;
                    // 是否匹配
                    if (!MatchKeyWord(msg, temp.Key))
                        continue;

                    method = _globalData.FuncTable[temp.Value].MethodInfo;
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 复杂匹配
        /// </summary>
        /// <param name="message">用户的输入字符串</param>
        /// <param name="attributes">KeyWordAttribute列表</param>
        /// <returns></returns>
        bool MatchKeyWord(string message,List<KeyWordAttribute> attributes) {
            string originalMessage = message;
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

        /// <summary>
        /// 发送回信
        /// </summary>
        /// <param name="post"></param>
        /// <param name="s"></param>
        /// <returns></returns>
        async ValueTask SendMessage(Post post,string s) {
            Console.WriteLine(s);
            var sender=(BaseMessage)post;
            Send send = new();
            send.MsgType=sender.MsgType;
            switch (send.MsgType) {
                case MessageType.Private:
                    send.UserID = ((PrivateMessage)post).UserID;
                    break;
                case MessageType.Group:
                    send.GroupID = ((GroupMessage)post).GroupID;
                    break;
                default:
                    return;
            }
            send.Msg=sender.Msg;
            SendMessage(new List<Send> { send });
        }
        async ValueTask SendMessage(Send send) {
            Console.WriteLine(send.ToString());
            SendMessage(new List<Send> { send });
        }
        async ValueTask SendMessage(List<Send> sends) {
            Console.WriteLine("List<Send>");
        }   
    }
}
