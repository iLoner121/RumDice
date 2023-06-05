﻿using Newtonsoft.Json;
using RumDice.Framework;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RumDice.Core {
    public class MessagePipeline : IMessagePipeline {
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


        public MessagePipeline(ICoreData coreData, IEventManager eventManager, IClientConnector clientConnector, IRumLogger logger) {
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
                    case AllType.PrivateMessage:
                        _eventManager.HandlePrivateMessage(item.post);
                        break;
                    case AllType.GroupMessage:
                        _eventManager.HandleGroupMessage(item.post);
                        break;
                    default:
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
                if (item.MsgType == MessageType.Group) {
                    _clientConnector.SendGroupMsg(item);
                    continue;
                }
                if(item.MsgType==MessageType.Private) {
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
            catch {

            }
        }

        public void RecvMsg(Post post,AllType type) {
            try {
                _recvQueue.Enqueue((type,post));
            }
            catch {
            }
        }

        public void RecvPrivateMsg(Post post) {
            RecvMsg(post, AllType.PrivateMessage);
        }

        public void RecvGroupMsg(Post post) {
            RecvMsg(post, AllType.GroupMessage);
        }
    }
}
