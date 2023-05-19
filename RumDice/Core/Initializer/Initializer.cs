using Microsoft.Extensions.Hosting;
using RumDice.Framework;
using RumDice.Module;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RumDice.Core {
    public class Initializer : IHostedService{
        readonly IGlobalData _globalData;
        readonly IServiceProvider _serviceProvider;
        readonly IServiceManager _serviceManager;
        public Initializer(IGlobalData globalData,
            IServiceProvider serviceProvider,
            IServiceManager serviceManager) {
            _globalData = globalData;
            _serviceProvider = serviceProvider;
            _serviceManager = serviceManager;
        }

        public async Task StartAsync(CancellationToken token) {
            // 初始化字典
            ValueTask t1 = _globalData.Initialize();
            // 初始化对象管理器
            ValueTask t2 =_serviceManager.Initialize();
            await t1;
            await t2;
            Console.WriteLine("初始化已完成");
            var service = (IEventManager)_serviceProvider.GetService(typeof(IEventManager));
            service?.HandleGroupMessage("hellopkpp");
            service?.HandleGroupMessage("ok");
            return;
        }

        public Task StopAsync(CancellationToken token) { 
            return Task.CompletedTask;
        }
    }
}
