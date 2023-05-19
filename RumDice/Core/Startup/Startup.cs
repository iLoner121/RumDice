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

namespace RumDice.Core.Startup {
    public class Startup {
        static async Task Main() {
            try {
                IHost host = Host.CreateDefaultBuilder()
                    .ConfigureServices(services =>
                    {
                        // 核心类
                        services.AddSingleton<IGlobalData,GlobalData>();
                        services.AddSingleton<IEventManager, EventManager>();
                        services.AddSingleton<IServiceManager, ServiceManager>();
                        
                        // host
                        services.AddHostedService<Initializer>();
                    })
                    .Build();
                await host.StartAsync();
            }
            catch {
                Console.WriteLine("生成失败");
            }
        }
    }
}
