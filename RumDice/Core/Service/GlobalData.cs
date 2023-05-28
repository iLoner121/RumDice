using Newtonsoft.Json;
using RumDice.Framework;
using RumDice.Module;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net.Http.Json;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace RumDice.Core {
    internal class GlobalData : IGlobalData {
        #region Setting
        public AppSetting Setting { get; set; } = new();
        #endregion

        #region MatchTable
        public Dictionary<string, string> ReplyTable { get; } = new();
        public Dictionary<List<KeyWordAttribute>, string> MatchTable { get; } = new();
        public Dictionary<string, MyMethodInfo> FuncTable { get; } = new();
        public int MinPriority { get; private set; } = 1;
        public int MaxPriority { get; private set; } = 5;
        #endregion


        public GlobalData() {

        }

        public async ValueTask Initialize() {
            await LoadAppSetting();
            await LoadInnerFunc();
        }
        public async ValueTask LoadAppSetting() {
            await Task.Delay(1);
            string path = Environment.CurrentDirectory;
            if (!File.Exists(path)) {
                string defaultSetting = JsonConvert.SerializeObject(Setting);
                Console.WriteLine(defaultSetting);
                //FileStream fs = new FileStream(path, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite) ;
                StreamWriter sw = new StreamWriter(path);
                sw.WriteLine(defaultSetting);
                sw.Close();
            } else {
                string settingJson = string.Empty;
                FileStream fs = new FileStream(path, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);
                StreamReader sr = new StreamReader(fs, Encoding.UTF8);
                settingJson = sr.ReadToEnd().ToString();
                AppSetting fileSetting = JsonConvert.DeserializeObject<AppSetting>(settingJson);
                if(fileSetting!= null) {
                    Setting = fileSetting;
                } else {
                    Console.WriteLine("解析AppSetting失败");
                }
            }
            

        }


        /// <summary>
        /// (重新)加载所有内置方法
        /// 会重新录入整个内置指令表
        /// </summary>
        public async ValueTask LoadInnerFunc() {
            await Task.Delay(1);
            var assemblys = Assembly.GetAssembly(typeof(IExample)).ExportedTypes
                .Where(t => t.IsInterface == true)
                .ToList();
            Console.WriteLine($"已读取到{assemblys.Count}个自定义服务");
            #region 识别接口匹配规则
            // 获取所有接口
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
                            MatchTable.Add(atts, method.Name);
                            FuncTable.Add(method.Name, new MyMethodInfo(method));
                            Console.WriteLine("*****接口导入成功");
                        }
                        catch {
                            Console.WriteLine("*****设置回复接口失败：重复的关键词或方法名");
                        }
                    }
                }
            }

            #endregion
            #region 识别reply规则


            #endregion
            #region 识别接口优先级
            // 内置复杂匹配
            // 识别没有设置优先级的接口
            foreach (var method in FuncTable.Values) {
                var priorityAttribute = (PriorityAttribute?)method.MethodInfo.GetCustomAttribute(typeof(PriorityAttribute));
                if(priorityAttribute != null) {
                    method.Priority=priorityAttribute.Priority;
                    Console.WriteLine($"方法{method.MethodInfo.Name}的优先级为{priorityAttribute.Priority}");
                    if(priorityAttribute.Priority<MinPriority) MinPriority=priorityAttribute.Priority;
                    if(priorityAttribute.Priority>MaxPriority) MaxPriority=priorityAttribute.Priority;
                } else {
                    method.Priority = 3;
                    Console.WriteLine($"方法{method.MethodInfo.Name}的优先级为3");
                }
            }
            foreach(var method in FuncTable.Values) {
                Console.WriteLine(method.MethodInfo.Name + " " + method.Priority);
            }
            // 该方法废弃，因为TypeDescriptor添加的attribute只能被TypeDescriptor找到
            /*
            foreach(var method in InnerFuncTable.Values) {
                if (method.GetCustomAttribute(typeof(PriorityAttribute)) is not PriorityAttribute)
                    continue;
                // 动态添加优先级3
                TypeDescriptor.AddAttributes(method, new PriorityAttribute(3));
                Console.WriteLine($"已将方法{method.Name}的优先级设置为3");
            }
            foreach(var method in InnerFuncTable) {
                Console.WriteLine($"方法{method.Key}的优先级为{TypeDescriptor.GetC}");
            }
            Console.WriteLine("优先级均设置完毕");
            */
            #endregion
            #region 设置接口是否为私聊
            foreach (var method in FuncTable.Values) {
                var isPrivateAttribute = (IsPrivateAttribute?)method.MethodInfo.GetCustomAttribute(typeof(IsPrivateAttribute));
                if (isPrivateAttribute != null) {
                    method.Scope = isPrivateAttribute.IsOnlyPrivate ? 3 : 2;
                } else {
                    method.Scope = 1;
                }
            }
            #endregion
            #region 识别仅内部调用接口
            foreach (var assembly in assemblys) {
                if (assembly.GetCustomAttribute(typeof(MyClassAttribute)) is not MyClassAttribute)
                    continue;
                // 获取内部方法
                var methods = assembly.GetMethods();
                foreach (var method in methods) {
                    foreach (var att in method.GetCustomAttributes()) {
                        if (att is not IsOnlyInternalAttribute a)
                            continue;
                        var myInfo = new MyMethodInfo(method);
                        myInfo.IsOnlyInternal = true;
                        try {
                            FuncTable.Add(method.Name, myInfo);
                            Console.WriteLine($"已设置内部服务：{method.Name}");
                        }
                        catch {
                            Console.WriteLine("设置内部服务失败");
                        }

                    }
                }
            }

            #endregion
        }

    }
}
