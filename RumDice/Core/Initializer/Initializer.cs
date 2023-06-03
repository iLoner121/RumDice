using EleCho.GoCqHttpSdk;
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
        readonly IClientConnector _clientConnector;
        readonly IDataCenter _dataCenter;
        readonly Tester _tester;
        public Initializer(ICoreData globalData,
            IServiceProvider serviceProvider,
            IServiceManager serviceManager,
            IClientConnector clientConnector,
            IDataCenter dataCenter,
            Tester tester) {
            _globalData = globalData;
            _serviceProvider = serviceProvider;
            _serviceManager = serviceManager;
            _clientConnector = clientConnector;
            _dataCenter = dataCenter;
            _tester = tester;
        }

        public async Task StartAsync(CancellationToken token) {
            // 初始化核心数据
            ValueTask t1 = _globalData.Initialize();
            // 初始化对象管理器
            ValueTask t2 =_serviceManager.Initialize();
            await t1;
            await t2;
            Console.WriteLine("初始化已完成");

            _dataCenter.Initialize(_globalData.RootDic+_globalData.Setting.FileConfig.RepositoryRoot);
            await _dataCenter.ScanFile();


            if (_globalData.Test == 0) {
                string uri = $"ws://{_globalData.Setting.ServerConfig.Location}:{_globalData.Setting.ServerConfig.Port}";
                await _clientConnector.RunServer(uri);
            } else {
                var service = (IEventManager)_serviceProvider.GetService(typeof(IEventManager));
                _tester.SetHandlePrivateMessage(service.HandlePrivateMessage);
                _tester.SetHandleGroupMessage(service.HandleGroupMessage);
                await _tester.RunTest();
            }
        }

        public Task StopAsync(CancellationToken token) { 
            return Task.CompletedTask;
        }
    }
}
