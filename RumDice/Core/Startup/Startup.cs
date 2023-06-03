using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RumDice.Framework;
using RumDice.Module;
using RumDice.Test;

namespace RumDice.Core.Startup {
    public class Startup {
        static async Task Main() {
            try {
                IHost host = Host.CreateDefaultBuilder()
                    .ConfigureServices(services =>
                    {
                        // 核心类
                        services.AddSingleton<ICoreData,CoreData>();
                        services.AddSingleton<IEventManager, EventManager>();
                        services.AddSingleton<IServiceManager, ServiceManager>();
                        services.AddSingleton<IClientConnector, ClientConnector>();
                        services.AddSingleton<IDataCenter, DataCenter>();
                        services.AddSingleton<Tester>();
                        
                        // host
                        services.AddHostedService<Initializer>();
                    })
                    .Build();
                await host.StartAsync();
                Console.WriteLine("!!!!!!!!!!!!!!!!!!!!!!");
            }
            catch {
                Console.WriteLine("生成失败");
            }
        }
    }
}
