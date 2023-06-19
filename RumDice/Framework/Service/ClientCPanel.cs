using CSScripting;
using Microsoft.Extensions.DependencyInjection;
using RumDice.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace RumDice.Framework {
    public class ClientCPanel : IClientCPanel {
        readonly IMsgTool _msgTool = new MsgTool();
        readonly IRumLogger _logger;
        readonly IServiceProvider _serviceProvider;
        readonly ICoreData _coreData;
        readonly IServiceManager _serviceManager;
        IMsgPipeline _messagePipeline;
        IEventManager _eventManager;

        public static ClientCPanel Instance;

        public ClientCPanel(IRumLogger logger, IServiceProvider serviceProvider, ICoreData coreData, IServiceManager serviceManager) {
            _logger = logger;
            _serviceProvider = serviceProvider;
            _coreData = coreData;
            _serviceManager = serviceManager;
            Instance = this;
        }


        public void SendMsg(string s, Post post) {
            BaseMsg sender = new();
            try {
                sender = (BaseMsg)post;
            }
            catch (Exception ex) {
                _logger.Error(ex, "该消息无法转换为Msg类型，无法生成回信包");
                return;
            }
            Send send = _msgTool.MakeSend(s, post);
            SendMsg(new List<Send> { send }, post);
        }

        public void SendMsg(Send send, Post post) {
            SendMsg(new List<Send> { send }, post);
        }

        public void SendMsg(List<Send> sends, Post post) {
            if (_messagePipeline == null) {
                _messagePipeline = (IMsgPipeline)_serviceProvider.GetService<IMsgPipeline>();
            }
            if (_eventManager == null) {
                _eventManager = (IEventManager)_serviceProvider.GetService<IEventManager>();
            }

            List<Send> sendquene = new();
            foreach (var send in sends) {
                send.Msg = UseMyService(send.Msg);
                var temps = SplitSend(send, post);
                sendquene.AddRange(temps);
            }
            _messagePipeline.Send(sendquene);
            _eventManager.HandleEvent(AllType.Send, post);
        }

        public async ValueTask SendMsgAsync(string s, Post post) {
            BaseMsg sender = new();
            try {
                sender = (BaseMsg)post;
            }
            catch (Exception ex) {
                _logger.Error(ex, "该消息无法转换为Msg类型，无法生成回信包");
                return;
            }
            Send send = _msgTool.MakeSend(s, post);
            await SendMsgAsync(new List<Send> { send }, post);
        }

        public async ValueTask SendMsgAsync(Send send, Post post) {
            if(send==null) return;
            await SendMsgAsync(new List<Send> { send }, post);
        }

        public async ValueTask SendMsgAsync(List<Send> sends, Post post) {
            if(sends==null || sends.Count==0) return;

            if (_messagePipeline == null) {
                _messagePipeline = (IMsgPipeline)_serviceProvider.GetService<IMsgPipeline>();
            }
            if(_eventManager == null) {
                _eventManager = (IEventManager)_serviceProvider.GetService<IEventManager>();
            }

            List<Send> sendquene = new();
            foreach (var send in sends) {
                send.Msg = UseMyService(send.Msg);
                var temps = SplitSend(send, post);
                sendquene.AddRange(temps);
            }
            _messagePipeline.Send(sendquene);
            await _eventManager.HandleEvent(AllType.Send, post);
        }

        public List<Send> SplitSend(Send send, Post post) {
            string msg = send.Msg;
            var splitMsg = msg.Split("(split)");
            List<Send> res = new MsgTool().MakeSend(splitMsg.ToList(), send);

            _logger.Debug("EventManager", "回复语句已分段完毕");
            return res;
        }

        public string UseMyService(string s) {
            _logger.Debug("EventManager", "最终回复语句开始处理");
            // 匹配
            string pattern = @"\{(?<name>.+?)\}";
            List<string>? matches = new();
            // 直到字符串内不含有任何花括号指令为止
            while (!(matches = Regex.Matches(s, pattern).Cast<Match>().Select(m => m.Groups["name"].Value).ToList()).IsEmpty()) {
                // 不放回抽牌的防重复表
                Dictionary<string, HashSet<string>> dic = new();
                // 遍历每一个匹配项
                foreach (var match in matches) {
                    // 匹配成功后将被替换掉的短语
                    string temp = "{" + match + "}";
                    if (match.Equals("split", StringComparison.OrdinalIgnoreCase)) {
                        Regex re = new Regex(temp);
                        s = re.Replace(s, "(split)", 1);
                        continue;
                    }
                    // 当前匹配指令
                    string tmatch = match;
                    // 替换的结果
                    string res = "";
                    // 是否可重复（放回）
                    bool isRepeat = false;
                    // 只有draw指令才判断是否放回
                    if (match.StartsWith("%")) {
                        isRepeat = true;
                        tmatch = match.Substring(1);
                    } else if (!match.StartsWith("draw:")) {
                        isRepeat = true;
                    }
                    // 尝试抽牌次数
                    int tryTimes = 0;
                    do {
                        // 产生匹配词
                        foreach (var method in _coreData.ServiceTable) {
                            if (tmatch.StartsWith(method.Key, StringComparison.OrdinalIgnoreCase)) {
                                string? rep = null;
                                try {
                                    var service = _serviceManager.GetService(method.Value.MethodInfo.DeclaringType);
                                    rep = method.Value.MethodInfo.Invoke(service, new object[] { tmatch }).ToString();
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
                        // 判断是否匹配结束
                        if (!isRepeat && !res.Equals("")) {
                            if (dic.ContainsKey(temp)) {
                                if (!dic[temp].Contains(res)) {
                                    dic[temp].Add(res);
                                    break;
                                }
                            } else {
                                dic.Add(temp, new HashSet<string> { res });
                                break;
                            }
                        } else {
                            break;
                        }
                        // 最多尝试十次
                        if (++tryTimes >= 10)
                            break;
                    } while (true);

                    _logger.Debug("EventManager", $"在回复语句中识别到 {temp} 指令，将替换为 \"{res}\"");
                    // 只替换最靠前的一个匹配项
                    Regex r = new Regex(temp);
                    s = r.Replace(s, res, 1);
                }
            }
            _logger.Debug("EventManager", "最终回复语句已经生成");
            return s;
        }

        public void SendOperation(Send send) {
            if (send == null)
                return;
            SendOperation(new List<Send> { send });
        }

        public void SendOperation(List<Send> sends) {
            if (sends == null)
                return;
            if (sends.Count == 0)
                return;
            _messagePipeline.Send(sends);
        }

        public async ValueTask SendOperationAsync(Send send) {
            if (send == null)
                return;
            await SendOperationAsync(new List<Send> { send });
        }

        public async ValueTask SendOperationAsync(List<Send> sends) {
            await Task.Delay(0);
            if (sends == null)
                return;
            if (sends.Count == 0)
                return;
            _messagePipeline.Send(sends);
        }

    }
}
