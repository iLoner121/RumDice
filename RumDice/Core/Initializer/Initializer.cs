﻿using EleCho.GoCqHttpSdk;
using Microsoft.Extensions.Hosting;
using Microsoft.VisualBasic;
using RumDice.Framework;
using RumDice.Module;
using RumDice.Test;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RumDice.Core {
    public class Initializer : IHostedService{
        readonly ICoreData _globalData;
        readonly IServiceProvider _serviceProvider;
        readonly IServiceManager _serviceManager;
        readonly QQClientConnector _qqConnector;
        readonly KookClientConnector _kookConnector;
        readonly IDataCenter _dataCenter;
        readonly IMsgPipeline _messagePipeline;
        readonly IRumLogger _logger;
        readonly Tester _tester;
        readonly IEventManager _eventManager;
        readonly IRumTimer _rumTimer;
        public Initializer(ICoreData globalData,
            IServiceProvider serviceProvider,
            IServiceManager serviceManager,
            QQClientConnector qqConnector,
            KookClientConnector kookConnector,
            IDataCenter dataCenter,
            Tester tester,
            IMsgPipeline messagePipeline,
            IRumLogger logger,
            IEventManager eventManager,
            IRumTimer rumTimer) {
            _globalData = globalData;
            _serviceProvider = serviceProvider;
            _serviceManager = serviceManager;
            _qqConnector = qqConnector;
            _kookConnector = kookConnector;
            _dataCenter = dataCenter;
            _tester = tester;
            _messagePipeline = messagePipeline;
            _logger = logger;
            _eventManager = eventManager;
            _rumTimer = rumTimer;
        }

        public async Task StartAsync(CancellationToken token) {
            #region 初始化
            // 初始化logger
            _logger.Initialize(_globalData.Setting,_globalData.RootDic);
            _logger.Info("Initializer", "启动开始");
            // 初始化核心数据
            await _globalData.Initialize();
            _logger.Info("Initializer", "核心数据已初始化");
            // 初始化对象管理器
            await _serviceManager.Initialize();
            _logger.Info("Initializer", "对象管理器已初始化");

            // 初始化文件管理器
            _dataCenter.Initialize(_globalData.Setting,_globalData.RootDic);
            // 扫描所有文件
            await _dataCenter.ScanAll();
            _logger.Info("Initializer", "文件系统已初始化");
            #endregion
            #region 设置信息
            // 加载回复词
            await _globalData.LoadReturnWord();
            _logger.Info("Initializer", "回复词表已初始化");
            #endregion
            await _messagePipeline.Initialize(_globalData.Test);
            _logger.Info("Initializer", "消息处理管线已初始化");

            ThreadPool.SetMinThreads(5, 5);
            ThreadPool.SetMaxThreads(_globalData.Setting.UserConfig.MaxThreadCount+10, _globalData.Setting.UserConfig.MaxThreadCount + 10);
            _logger.Info("Initializer", "线程池已初始化");

            await _eventManager.HandleEvent(AllType.Start, new Post());
            _logger.Info("Initializer", "监听启动的服务已执行完毕");

            await _rumTimer.StartTimer();
            _logger.Info("Initializer", "定时器已初始化，10秒后开始计时");

            _logger.Info("Initializer", "初始化已完成，3秒后开始启动服务");
            Task.Delay(3000).Wait();

            List<Task> tasks = new();
            if (_globalData.Test == 0) {
                if (_globalData.Setting.ServerConfig.QQbot) {
                    _logger.Info("Initializer", "开始启动QQ客户端服务");
                    string uri = $"ws://{_globalData.Setting.ServerConfig.Location}:{_globalData.Setting.ServerConfig.Port}";
                    tasks.Add(_qqConnector.RunServerAsync(uri).AsTask());
                }
                if (_globalData.Setting.ServerConfig.KOOKbot) {
                    _logger.Info("Initializer", "开始启动Kook客户端服务");
                    tasks.Add(_kookConnector.RunServerAsync(_globalData.Setting.ServerConfig.KookToken).AsTask());
                }
            } else {
                _logger.Info("Initializer", "开始启动测试程序");
                _tester.SetHandlePrivateMessage(_messagePipeline.RecvPrivateMsg);
                _tester.SetHandleGroupMessage(_messagePipeline.RecvGroupMsg);
                await _tester.RunTest();
                return;
            }
            Task.WaitAll(tasks.ToArray());
        }

        public Task StopAsync(CancellationToken token) {
            _logger.Info("Initializer", "服务已关闭");
            return Task.CompletedTask;
        }
    }
}
