using Newtonsoft.Json;
using RumDice.Framework;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RumDice.Core {
    public class MsgPipeline : IMsgPipeline {
        ConcurrentQueue<Send> _sendQueue = new();
        ConcurrentQueue<(AllType type,Post post)> _recvQueue = new();

        static System.Threading.Timer _recvTimer;
        static System.Threading.Timer _sendTimer;
        static volatile bool _isSending = false;
        static volatile bool _isRecving = false;

        // 发送信息的间隔
        int _minDuration;
        int _maxDuration;
        // 计时器唤醒间隔
        int _recvWakeDuration;
        int _sendWakeDuration;

        readonly ICoreData coreData;
        readonly IEventManager _eventManager;
        readonly IClientConnector _clientConnector;
        readonly IRumLogger _logger;

        int _mode;

        Random random = new Random();


        public MsgPipeline(ICoreData coreData, IEventManager eventManager, IClientConnector clientConnector, IRumLogger logger) {
            this.coreData = coreData;
            _eventManager = eventManager;
            _clientConnector = clientConnector;
            _logger = logger;
        }


        public async ValueTask Initialize(int mode) {
            _minDuration = coreData.Setting.UserConfig.MinDuration;
            _maxDuration = coreData.Setting.UserConfig.MaxDuration;
            _recvWakeDuration= coreData.Setting.UserConfig.RecvWakeDuration;
            _sendWakeDuration=coreData.Setting.UserConfig.SendWakeDuration;
            _recvTimer = new System.Threading.Timer(HandleRecvQueue, null, 1000, _recvWakeDuration);
            _sendTimer = new System.Threading.Timer(HandleSendQueue, null, 1000, _sendWakeDuration);
            _mode=mode;
        }

        // 处理消息队列
        void HandleRecvQueue(object obj) {
            if(_isRecving) {
                return;
            } else {
                _isRecving = true;
            }
            // 出列
            int n = 0;
            while (_recvQueue.TryDequeue(out var item)) {
                // 等待线程池空闲
                if (ThreadPool.ThreadCount > coreData.Setting.UserConfig.MaxThreadCount) {
                    Task.Delay(100).Wait();
                }
                // 转发消息
                switch (item.type) {
                    case AllType.PrivateMsg:
                        _eventManager.HandleEvent(AllType.PrivateMsg,item.post);
                        _eventManager.HandleEvent(AllType.Msg, item.post);
                        _eventManager.HandleEvent(AllType.Post, item.post);
                        _eventManager.HandlePrivateMessage(item.post);
                        break;
                    case AllType.GroupMsg:
                        _eventManager.HandleEvent(AllType.GroupMsg, item.post);
                        _eventManager.HandleEvent(AllType.Msg, item.post);
                        _eventManager.HandleEvent(AllType.Post, item.post);
                        _eventManager.HandleGroupMessage(item.post);
                        break;
                    default:
                        _eventManager.HandleEvent(AllType.Post,item.post);
                        _eventManager.HandleEvent(item.type, item.post);
                        break;
                }
            }
            _isRecving = false;
        }

        void HandleSendQueue(object obj) {
            if(_isSending) {
                return;
            } else {
                _isSending = true;
            }
            while (_sendQueue.TryDequeue(out var item)) {
                Task.Delay(random.Next(_minDuration, _maxDuration + 1)).Wait();
                if (_mode == 1) {
                    _logger.Info("MessagePipeline", "发信：" + JsonConvert.SerializeObject(item));
                }
                if (item.MsgType == MsgType.Group) {
                    _clientConnector.SendGroupMsg(item);
                    continue;
                }
                if(item.MsgType==MsgType.Private) {
                    _clientConnector.SendPrivateMsg(item);
                    continue;
                }
                if(item.UserID!=0) {
                    _clientConnector.SendPrivateMsg(item);
                    continue;
                }
                if(item.GroupID!=0) {
                    _clientConnector.SendGroupMsg(item);
                    continue;
                }
            }
            _isSending = false;
        }


        public void SendMsg(List<Send> sends) {
            try {
                foreach (var send in sends) {
                    _sendQueue.Enqueue(send);
                }
            }
            catch(Exception ex) {
                _logger.Error(ex, "加入发信队列失败");
            }
        }

        public void RecvMsg(Post post,AllType type) {
            try {
                _recvQueue.Enqueue((type,post));
            }
            catch (Exception ex) {
                _logger.Error(ex, "加入收信队列失败");
            }
        }

        public void RecvPrivateMsg(Post post) {
            RecvMsg(post, AllType.PrivateMsg);
        }

        public void RecvGroupMsg(Post post) {
            RecvMsg(post, AllType.GroupMsg);
        }

        public void RecvFriendRecallNotice(Post post) {
            RecvMsg(post, AllType.FriendRecall);
        }

        public void RecvGroupRecallNotice(Post post) {
            RecvMsg(post, AllType.GroupRecall);
        }

        public void RecvGroupIncreaseNotice(Post post) {
            RecvMsg (post, AllType.GroupIncrease);
        }

        public void RecvGroupDecreaseNotice(Post post) {
            RecvMsg(post, AllType.GroupDecrease);
        }

        public void RecvGroupAdminNotice(Post post) {
            RecvMsg(post, AllType.GroupAdmin);
        }

        public void RecvGroupBanNotice(Post post) {
            RecvMsg(post, AllType.GroupBan);
        }

        public void RecvFriendAddNotice(Post post) {
            RecvMsg(post, AllType.FriendAdd);
        }

        public void RecvGroupPokeNotice(Post post) {
            RecvMsg(post, AllType.GroupPoke);
        }

        public void RecvHonorNotice(Post post) {
            RecvMsg(post, AllType.Honor);
        }

        public void RecvTitleNotice(Post post) {
            RecvMsg(post, AllType.Title);
        }

        public void RecvGroupCardNotice(Post post) {
            RecvMsg(post, AllType.GroupCard);
        }

        public void RecvFriendRequest(Post post) {
            RecvMsg(post, AllType.FriendRequest);
        }

        public void RecvGroupRequest(Post post) {
            RecvMsg(post, AllType.GroupRequest);
        }

    }
}
