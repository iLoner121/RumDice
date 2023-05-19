using RumDice.Framework;
using RumDice.Module;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace RumDice.Core {
    internal class GlobalData : IGlobalData {
        #region MatchTable
        public Dictionary<string, string> InnerReplyTable { get; }
        public Dictionary<string, string> PluginReplyTable { get; }
        public Dictionary<List<KeyWordAttribute>,MethodInfo> InnerMatchTable { get; }
        public Dictionary<List<KeyWordAttribute>,MethodInfo> PluginMatchTable { get; }
        public Dictionary<string,MethodInfo> InnerFuncTable { get; }
        public Dictionary<string,MethodInfo> PluginFuncTable { get; }
        #endregion


        public GlobalData() {
            // 方法匹配字典
            InnerReplyTable = new();
            PluginReplyTable = new();
            InnerMatchTable = new();
            PluginMatchTable = new();
            InnerFuncTable = new();
            PluginFuncTable = new();
        }

        public async ValueTask Initialize() {
            await LoadInnerFunc();
        }

        /// <summary>
        /// (重新)加载所有内置方法
        /// 会重新录入整个内置指令表
        /// </summary>
        public async ValueTask LoadInnerFunc() {
            await Task.Delay(1);
            // 获取所有接口
            var assemblys = Assembly.GetAssembly(typeof(IExample)).ExportedTypes
                .Where(t => t.IsInterface==true)
                .ToList();
            Console.WriteLine($"已读取到{assemblys.Count}个自定义服务");
            foreach (var assembly in assemblys) {
                if (assembly.GetCustomAttribute(typeof(MyClassAttribute)) is not MyClassAttribute)
                    continue;

                Console.WriteLine($"**已识别到回复服务：{assembly.Name}");

                // 获取内部方法
                var methods = assembly.GetMethods();
                foreach (var method in methods) {
                    var atts = new List<KeyWordAttribute>();
                    foreach (var att in method.GetCustomAttributes()) {
                        if(att is not KeyWordAttribute a) 
                            continue;
                        atts.Add(a);
                    }
                    if (atts.Count > 0) {
                        Console.WriteLine($"****已识别回复接口：{method.Name}");
                        try {
                            InnerMatchTable.Add(atts, method);
                            InnerFuncTable.Add(method.Name, method);
                            Console.WriteLine("*****接口导入成功");
                        }
                        catch {
                            Console.WriteLine("*****设置回复接口失败：重复的关键词或方法名");
                        }
                    }
                    // TODO: 加入innerReplyTable
                    
                    
                }

                
            }
        }

    }
}
