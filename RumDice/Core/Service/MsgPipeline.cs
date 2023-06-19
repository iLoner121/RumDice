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
        readonly QQClientConnector _qqConnector;
        readonly KookClientConnector _kookConnector;
        readonly IRumLogger _logger;

        int _mode;

        Random random = new Random();


        public MsgPipeline(ICoreData coreData, IEventManager eventManager, QQClientConnector qqConnector, IRumLogger logger, KookClientConnector kookConnector) {
            this.coreData = coreData;
            _eventManager = eventManager;
            _qqConnector = qqConnector;
            _logger = logger;
            _kookConnector = kookConnector;
            
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
                    _logger.Info("MsgPipeline", "发信：" + JsonConvert.SerializeObject(item));
                    continue;
                }
                if (HandleOneBotOperation(item, item.BotType))
                    continue;
                if (HandleSendMsg(item, item.BotType))
                    continue;
            }
            _isSending = false;
        }

        bool HandleSendMsg(Send send,BotType botType) {
            IBaseClient baseClient = null;
            switch (botType) {
                case BotType.QQbot:
                    baseClient= _qqConnector;
                    break;
                case BotType.KOOKbot:
                    baseClient= _kookConnector;
                    break;
                default:
                    return false;
            }
            if (send.MsgType == MsgType.Group) {
                baseClient.SendGroupMsg(send);
                return true;
            }
            if (send.MsgType == MsgType.Private) {
                baseClient.SendPrivateMsg(send);
                return true;
            }
            if (send.UserID != 0) {
                baseClient.SendPrivateMsg(send);
                return true;
            }
            if (send.GroupID != 0) {
                baseClient.SendGroupMsg(send);
                return true;
            }
            return false;
        }

        bool HandleOneBotOperation(Send send,BotType botType) {
            IOnebotClient onebotClient = null;
            OneBotAction action;
            switch (botType) {
                case BotType.QQbot:
                    if (send is not QQSend s)
                        return false;
                    onebotClient= _qqConnector;
                    action = s.Action;
                    break;
                default:
                    return false;
            }
            _logger.Info("MsgPipeline", $"向OneBot平台{botType}发送控制命令：{action}");
            switch (action) {
                case OneBotAction.RecallMsg:
                    onebotClient.RecallMsg(send);
                    break;
                case OneBotAction.KickGroupMember:
                    onebotClient.KickGroupMember(send);
                    break;
                case OneBotAction.BanGroupMember:
                    onebotClient.BanGroupMember(send);
                    break;
                case OneBotAction.BanGroupAll:
                    onebotClient.BanGroupAll(send);
                    break;
                case OneBotAction.CancelBanGroupMember:
                    onebotClient.CancelBanGroupMember(send);
                    break;
                case OneBotAction.CancelBanGroupAll:
                    onebotClient.CancelBanGroupAll(send);
                    break;
                case OneBotAction.SetGroupAdmin:
                    onebotClient.SetGroupAdmin(send);
                    break;
                case OneBotAction.SetGroupCard:
                    onebotClient.SetGroupCard(send);
                    break;
                case OneBotAction.SetGroupName:
                    onebotClient.SetGroupName(send);
                    break;
                case OneBotAction.LeaveGroup:
                    onebotClient.LeaveGroup(send);
                    break;
                case OneBotAction.SetGroupTitle:
                    onebotClient.SetGroupTitle(send);
                    break;
                case OneBotAction.AcceptFriend:
                    onebotClient.AcceptFriend(send);
                    break;
                case OneBotAction.AcceptGroup:
                    onebotClient.AcceptGroup(send);
                    break;
                case OneBotAction.DeleteFriend:
                    onebotClient.DeleteFriend(send);
                    break;
                case OneBotAction.DeleteNonFriend:
                    onebotClient.DeleteNonFriend(send);
                    break;
                case OneBotAction.UploadGroupFile:
                    onebotClient.UploadGroupFile(send);
                    break;
                default:
                    return false;
            }
            return true;
        }

        public void Send(List<Send> sends) {
            try {
                foreach (var send in sends) {
                    _sendQueue.Enqueue(send);
                }
            }
            catch(Exception ex) {
                _logger.Error(ex, "加入发信队列失败");
            }
        }


        public void Recv(Post post,AllType type) {
            try {
                _recvQueue.Enqueue((type,post));
            }
            catch (Exception ex) {
                _logger.Error(ex, "加入收信队列失败");
            }
        }

        public void RecvPrivateMsg(Post post) {
            Recv(post, AllType.PrivateMsg);
        }

        public void RecvGroupMsg(Post post) {
            Recv(post, AllType.GroupMsg);
        }

        public void RecvFriendRecallNotice(Post post) {
            Recv(post, AllType.FriendRecall);
        }

        public void RecvGroupRecallNotice(Post post) {
            Recv(post, AllType.GroupRecall);
        }

        public void RecvGroupIncreaseNotice(Post post) {
            Recv (post, AllType.GroupIncrease);
        }

        public void RecvGroupDecreaseNotice(Post post) {
            Recv(post, AllType.GroupDecrease);
        }

        public void RecvGroupAdminNotice(Post post) {
            Recv(post, AllType.GroupAdmin);
        }

        public void RecvGroupBanNotice(Post post) {
            Recv(post, AllType.GroupBan);
        }

        public void RecvFriendAddNotice(Post post) {
            Recv(post, AllType.FriendAdd);
        }

        public void RecvGroupPokeNotice(Post post) {
            Recv(post, AllType.GroupPoke);
        }

        public void RecvHonorNotice(Post post) {
            Recv(post, AllType.Honor);
        }

        public void RecvTitleNotice(Post post) {
            Recv(post, AllType.Title);
        }

        public void RecvGroupCardNotice(Post post) {
            Recv(post, AllType.GroupCard);
        }

        public void RecvFriendRequest(Post post) {
            Recv(post, AllType.FriendRequest);
        }

        public void RecvGroupRequest(Post post) {
            Recv(post, AllType.GroupRequest);
        }

    }
}
