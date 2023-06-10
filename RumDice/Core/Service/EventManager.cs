using CSScripting;
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
        readonly IServiceProvider _serviceProvider;
        readonly IRumLogger _logger;

        readonly int _minPriority;
        readonly int _maxPriority;

        IMsgPipeline _messagePipeline;


        public EventManager(ICoreData globalData,
            IServiceManager serviceManager,
            IServiceProvider serviceProvider,
            IRumLogger logger) {
            _globalData = globalData;
            _serviceManager = serviceManager;

            _minPriority = globalData.MinPriority;
            _maxPriority = globalData.MaxPriority;
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        public async void HandleEvent(AllType type, Post post) {
            if (!_globalData.ListenerTable.ContainsKey(type)) {
                _logger.Debug("EventManager", "该事件无对应监听");
                return;
            }
            _logger.Info("EventManager", $"已接到{type.ToString()}类型事件，开始分发");
            foreach(var mi in _globalData.ListenerTable[type]) {
                Invoke(mi.MethodInfo, post);
            }
            return;
        }


        public async void HandlePrivateMessage(Post post) {
            await Task.Delay(0);
            _logger.Debug("EventManager","消息已接收->私聊");
            var baseMsg = (BaseMsg)post;
            MethodInfo method;
            bool isMatch = MatchFunc(baseMsg.Msg, 1, out method);

            if (isMatch) {
                _logger.Info("EventManager", "指令已匹配->私聊");
                Invoke(method, post);
            } else {
                _logger.Debug("EventManager", "指令未匹配->私聊");
            }
        }

        public async void HandleGroupMessage(Post post) {
            await Task.Delay(0);
            _logger.Debug("EventManager", "消息已接收->群聊");
            var baseMsg = (BaseMsg)post;
            MethodInfo method;
            bool isMatch = MatchFunc(baseMsg.Msg, 3 , out method);

            if (isMatch) {
                _logger.Info("EventManager", "指令已匹配->群聊");
                Invoke(method, post);
            } else {
                _logger.Debug("EventManager", "指令未匹配->群聊");
            }
        }

        /// <summary>
        /// 调用匹配到的服务
        /// </summary>
        /// <param name="method"></param>
        /// <param name="post"></param>
        /// <returns></returns>
        async ValueTask Invoke(MethodInfo method, Post post) {
            try {
                var service = _serviceManager.GetService(method.DeclaringType);

                if (service == null) {
                    _logger.Warn("EventManager", $"获取回复接口服务失败：{method.Name}");
                    return;
                }

                var res = method.Invoke(service, new object[] { post });
                Console.WriteLine("1");
                if (res == null) {
                    _logger.Debug("EventManager", $"未获取该回复接口的返回值：{method.Name}");
                    return;
                }
                Console.WriteLine(res.ToString()+" "+res.GetType());
                if (res is string s) {
                    await SendMessage(post, s);
                    return;
                }
                if (res is Send send) {
                    await SendMessage(send);
                    return;
                }
                if (res is List<Send> sends) {
                    await SendMessage(sends);
                    return;
                }
                _logger.Warn("EventManager", $"该回复接口具备错误的返回格式：{method.Name}");
                return;
            }
            catch(Exception e) {
                _logger.Error(e, $"调用接口服务失败：{method.Name}");
            }
        }

        /// <summary>
        /// 匹配全过程
        /// </summary>
        /// <param name="scopeCheck">不符合的scope</param>
        /// <param name="method">返回的method</param>
        /// <returns></returns>
        public bool MatchFunc(string msg,int scopeCheck,out MethodInfo method) {
            method = null;

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
            // TODO: 优化代码可读性和效率
            if (message == null || message.IsEmpty())
                return false;

            // 保存原始字符串
            string originalMessage = message;

            // 匹配标识符
            bool c1 = true, c2 = true, c3 = true, c4 = true;
            foreach (KeyWordAttribute attribute in attributes) {
                // 判断是否为正则匹配，抵消其他操作
                if (attribute.IsRegex) {
                    if (Regex.IsMatch(attribute.KeyWord, message)) {
                        return true;
                    } else {
                        continue;
                    }
                }

                // 判断是否大小写敏感，生成对应的匹配素材
                string kys = attribute.KeyWord;
                if(!attribute.IsCaseSensitive) {
                    message = originalMessage.ToLower();
                    kys = kys.ToLower();
                }

                string[] keywords = kys.Split(' ');
                string[] messageSplit = message.Split(' ');
                c1 = true; c2 = true; c3 = true; c4=true;
                foreach(string keyword in keywords) {
                    // 是否符合模糊匹配
                    if (c1) {
                        if (message.Contains(keyword)) {
                            if (attribute.IsDivided) {
                                if(messageSplit.Contains(keyword)) {
                                    c1 = false;
                                }
                            }else {
                                c1 = false;
                            }
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
                            if (attribute.IsDivided) {
                                if (messageSplit.Contains(keyword)) {
                                    c3 = false;
                                }
                            } else {
                                c3 = false;
                            }
                        }
                    }
                    // 是否符合后缀
                    if (attribute.IsSuffix && c4) {
                        if(message.EndsWith(keyword)) {
                            if (attribute.IsDivided) {
                                if (messageSplit.Contains(keyword)) {
                                    c4 = false;
                                }
                            } else {
                                c4 = false;
                            }
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
            var sender=(BaseMsg)post;
            Send send = new();
            send.MsgType=sender.MsgType;
            send.Msg = s;
            switch (send.MsgType) {
                case MsgType.Private:
                    send.UserID = ((PrivateMsg)post).UserID;
                    break;
                case MsgType.Group:
                    send.GroupID = ((GroupMsg)post).GroupID;
                    break;
                default:
                    return;
            }
            SendMessage(new List<Send> { send });
        }
        async ValueTask SendMessage(Send send) {
            SendMessage(new List<Send> { send });
        }
        async ValueTask SendMessage(List<Send> sends) {

            if(_messagePipeline==null) {
                _messagePipeline = (IMsgPipeline)_serviceProvider.GetService<IMsgPipeline>();
            }
            foreach(var send in sends) {
                send.Msg = UseMyService(send.Msg);
            }
            _messagePipeline.SendMsg(sends);
        }   

        /// <summary>
        /// 替换字符串中的变量
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        string UseMyService(string s) {
            _logger.Debug("EventManager", "最终回复语句开始处理");
            string pattern = @"\{(?<name>.+?)\}";
            HashSet<string>? matches = new HashSet<string>();
            int count = 0;
            while (!(matches=Regex.Matches(s, pattern).Cast<Match>().Select(m => m.Groups["name"].Value).ToHashSet()).IsEmpty()) {
                foreach (var match in matches) {
                    string temp = "{" + match + "}";
                    string res = "";
                    foreach (var method in _globalData.ServiceTable) {
                        if (method.Key.StartsWith(match, StringComparison.OrdinalIgnoreCase)) {
                            string? rep = null;

                            try {
                                var service = _serviceManager.GetService(method.Value.MethodInfo.DeclaringType);
                                rep = method.Value.MethodInfo.Invoke(service, new object[] { match }).ToString();
                            }
                            catch (Exception ex) {
                                _logger.Error(ex, "内置服务调用失败");
                            }

                            if (rep == null) {
                                continue;
                            }
                            if (rep is string) {
                                res = rep;
                                break;
                            }
                        }
                    }
                    _logger.Debug("EventManager", $"在回复语句中识别到 {temp} 指令，将替换为 \"{res}\"");
                    s = s.Replace(temp, res);
                }
            }
            _logger.Debug("EventManager", "最终回复语句已经生成");
            return s;
        }
    }
}
