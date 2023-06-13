using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
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
                        services.AddSingleton<IMsgPipeline, MsgPipeline>();
                        services.AddSingleton<IRumLogger, RumLogger>();
                        services.AddSingleton<Tester>();
                        
                        // host
                        services.AddHostedService<Initializer>();
                    })
                    .Build();
                host.RunAsync();
                //await host.StartAsync();
                while (true) {
                    //cccc;
                }
                ConsoleColor c = Console.BackgroundColor;
                Console.ForegroundColor = ConsoleColor.DarkGreen;
                Console.BackgroundColor = c;
                Console.Write($@"[ {DateTime.Now.ToString()} ] | ");
                Console.ForegroundColor = ConsoleColor.Black;
                Console.BackgroundColor = ConsoleColor.DarkGreen;
                Console.Write(@" [Info] ");
                Console.ForegroundColor = ConsoleColor.DarkGreen;
                Console.BackgroundColor = c;
                Console.WriteLine($"[RumDice]运行结束");
                Console.ForegroundColor = ConsoleColor.White;
                Console.BackgroundColor = c;
            }
            catch {
                ConsoleColor c = Console.BackgroundColor;
                Console.ForegroundColor= ConsoleColor.Red;
                Console.BackgroundColor= c;
                Console.Write($@"[ {DateTime.Now.ToString()} ] | ");
                Console.ForegroundColor = ConsoleColor.Black;
                Console.BackgroundColor = ConsoleColor.Red;
                Console.Write(@" [Fatal] ");
                Console.ForegroundColor = ConsoleColor.Red;
                Console.BackgroundColor = c;
                Console.WriteLine($"[RumDice]生成失败");
                Console.ForegroundColor = ConsoleColor.White;
                Console.BackgroundColor = c;
            }
        }
    }
}
