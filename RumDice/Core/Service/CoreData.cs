using Newtonsoft.Json;
using RumDice.Framework;
using RumDice.Module;
using System;
using System.Collections;
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
    public class CoreData : ICoreData {
        public static ICoreData? Instance { get; private set; }
        public int Mode { get; } = 0;

        public int Test { get; } = 0;
        #region Setting
        public string RootDic { get; set; }

        public AppSetting Setting { get; set; } = new();
        #endregion

        #region MatchTable
        public Dictionary<List<KeyWordAttribute>, string> MatchTable { get; } = new();
        public Dictionary<string, MyMethodInfo> FuncTable { get; } = new();
        public Dictionary<string, MyMethodInfo> ServiceTable { get; } = new();
        public Dictionary<AllType, List<MyMethodInfo>> ListenerTable { get; } = new();
        public int MinPriority { get; private set; } = 1;
        public int MaxPriority { get; private set; } = 5;
        #endregion


        readonly IDataCenter _dataCenter;
        readonly IRumLogger _logger;


        public CoreData(IDataCenter dataCenter, IRumLogger logger) {
            switch (Mode) {
                case 0:
                    string curDic = Environment.CurrentDirectory;
                    DirectoryInfo dicInfo = new DirectoryInfo(curDic);
                    RootDic = dicInfo.Parent.Parent.Parent.FullName;
                    break;
                case 1:
                    RootDic = Environment.CurrentDirectory;
                    break;
                default:
                    break;
            }
            // 载入appsetting
            _dataCenter = dataCenter;
            Instance = this;

            //LoadAppSetting().AsTask().Wait();

            string path = RootDic + "\\AppSetting.json";
            if (!File.Exists(path)) {
                string defaultSetting = JsonConvert.SerializeObject(Setting);
                FileStream fs = new FileStream(path, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);
                fs.Seek(0, SeekOrigin.Begin);
                fs.SetLength(0);
                StreamWriter sw = new StreamWriter(fs, Encoding.UTF8);
                sw.WriteLine(defaultSetting);
                sw.Close();
            } else {
                string settingJson = string.Empty;
                FileStream fs = new FileStream(path, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);
                StreamReader sr = new StreamReader(fs, Encoding.UTF8);
                settingJson = sr.ReadToEnd().ToString();
                AppSetting fileSetting = JsonConvert.DeserializeObject<AppSetting>(settingJson);
                if (fileSetting != null) {
                    Setting = fileSetting;
                } else {
                    Console.WriteLine("解析AppSetting失败");
                }
            }
            _logger = logger;
        }

        public async ValueTask Initialize() {
            await LoadAppSetting();
            await LoadInnerFunc();
        }
        public async ValueTask LoadAppSetting() {
            await Task.Delay(0);
            // 生成路径
            string path = RootDic + "\\AppSetting.json";
            _logger.Debug("CoreData", $"当前AppSetting路径为 {path}");

            if (!File.Exists(path)) {
                _logger.Debug("CoreData", "AppSetting文件不存在，正在自动生成");
                string defaultSetting = JsonConvert.SerializeObject(Setting);
                _logger.Debug("CoreData", $"默认AppSetting的内容为 {defaultSetting}");
                FileStream fs = new FileStream(path, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);
                fs.Seek(0, SeekOrigin.Begin);
                fs.SetLength(0);
                StreamWriter sw = new StreamWriter(fs, Encoding.UTF8);
                sw.WriteLine(defaultSetting);
                sw.Close();
            } else {
                string settingJson = string.Empty;
                FileStream fs = new FileStream(path, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);
                StreamReader sr = new StreamReader(fs, Encoding.UTF8);
                settingJson = sr.ReadToEnd().ToString();
                try {
                    AppSetting fileSetting = JsonConvert.DeserializeObject<AppSetting>(settingJson);
                    Setting = fileSetting;
                }
                catch (Exception ex){
                    _logger.Fatal(ex, "解析AppSetting失败");
                }
            }
            _logger.Debug("CoreData", "AppSetting加载完成");
        }


        /// <summary>
        /// (重新)加载所有内置方法
        /// 会重新录入整个内置指令表
        /// </summary>
        public async ValueTask LoadInnerFunc() {
            await Task.Delay(0);
            var assemblys = Assembly.GetAssembly(typeof(IExample)).ExportedTypes
                .ToList();

            _logger.Debug("CoreData", $"*已读取到{assemblys.Count}个类和接口");
            #region 识别接口匹配规则
            // 获取所有接口
            foreach (var assembly in assemblys) {
                if (assembly.GetCustomAttribute(typeof(MyClassAttribute)) is not MyClassAttribute)
                    continue;

                _logger.Debug("CoreData", $"**已读取到自定义服务类：{assembly.Name}");

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
                        try {
                            MatchTable.Add(atts, $"{assembly.FullName}.{method.Name}");
                            FuncTable.Add($"{assembly.FullName}.{method.Name}", new MyMethodInfo(method));
                            _logger.Debug("CoreData", $"***已导入回复接口：{method.Name}");
                        }
                        catch (Exception ex){
                            _logger.Error(ex, "***导入回复接口失败：重复的关键词或方法名称");
                            if(MatchTable.ContainsKey(atts))
                                MatchTable.Remove(atts);
                            if (FuncTable.ContainsKey($"{assembly.FullName}.{method.Name}"))
                                FuncTable.Remove($"{assembly.FullName}.{method.Name}");
                        }
                    }
                }
            }
            #endregion
            #region 识别reply规则
            foreach (var assembly in assemblys) {
                if (assembly.GetCustomAttribute(typeof(MyClassAttribute)) is not MyClassAttribute)
                    continue;
                // 获取内部方法
                var methods = assembly.GetMethods();
                foreach (var method in methods) {
                    foreach (var att in method.GetCustomAttributes()) {
                        if (att is not ReplyAttribute a)
                            continue;
                        try {
                            if (!FuncTable.ContainsKey($"{assembly.FullName}.{method.Name}")) {
                                var myInfo = new MyMethodInfo(method);
                                myInfo.Priority = 0;
                                FuncTable.Add($"{assembly.FullName}.{method.Name}", myInfo);
                            }
                            var k = new List<KeyWordAttribute>() { new KeyWordAttribute(a.Reply, isFullMatch:true) };
                            MatchTable.Add(k, $"{assembly.FullName}.{method.Name}");
                            _logger.Debug("CoreData", "-*-导入Reply接口成功");
                        }
                        catch (Exception ex) {
                            _logger.Error(ex, "-*-导入Reply接口失败");
                        }
                    }
                }
            }
            #endregion
            #region 设置自动前缀指令
            foreach (var assembly in assemblys) {
                if (assembly.GetCustomAttribute(typeof(MyClassAttribute)) is not MyClassAttribute)
                    continue;
                // 获取内部方法
                var methods = assembly.GetMethods();
                foreach (var method in methods) {
                    foreach (var att in method.GetCustomAttributes()) {
                        if (att is not PrefixMatchAttribute a)
                            continue;
                        try {
                            MatchTable.Add(
                                new List<KeyWordAttribute>() { 
                                    new KeyWordAttribute($".{a.Prefix}", isPrefix: true)
                                }, $"{assembly.FullName}.{method.Name}");
                            MatchTable.Add(
                                new List<KeyWordAttribute>() {
                                    new KeyWordAttribute($"。{a.Prefix}", isPrefix: true)
                                }, $"{assembly.FullName}.{method.Name}");
                            MatchTable.Add(
                                new List<KeyWordAttribute>() {
                                    new KeyWordAttribute($"{a.Prefix}", isPrefix: true,isDivided:true)
                                }, $"{assembly.FullName}.{method.Name}");
                            _logger.Debug("CoreData", $"---已导入前缀指令：{a.Prefix}->{method.Name}");
                            if (FuncTable.ContainsKey($"{assembly.FullName}.{method.Name}")) {
                                return;
                            }
                            var myInfo = new MyMethodInfo(method);
                            FuncTable.Add($"{assembly.FullName}.{method.Name}", myInfo);
                        }
                        catch (Exception ex) {
                            _logger.Error(ex, "---导入前缀指令失败");
                        }
                    }
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
                        if (att is not MyServiceAttribute a)
                            continue;
                        var myInfo = new MyMethodInfo(method);
                        try {
                            ServiceTable.Add(a.command, myInfo);
                            _logger.Debug("CoreData", $"-=-已导入内部服务：{a.command}->{method.Name}");
                        }
                        catch (Exception ex) {
                            _logger.Error(ex, "-=-导入内置服务失败");
                        }
                    }
                }
            }
            #endregion
            #region 识别接口优先级
            // 内置复杂匹配
            // 识别没有设置优先级的接口
            foreach (var method in FuncTable.Values) {
                var priorityAttribute = (PriorityAttribute?)method.MethodInfo.GetCustomAttribute(typeof(PriorityAttribute));
                if(priorityAttribute != null) {
                    method.Priority=priorityAttribute.Priority;
                    _logger.Debug("CoreData", $"===方法{method.MethodInfo.Name}的优先级为{priorityAttribute.Priority}");
                    if (priorityAttribute.Priority<MinPriority) MinPriority=priorityAttribute.Priority;
                    if(priorityAttribute.Priority>MaxPriority) MaxPriority=priorityAttribute.Priority;
                } else {
                    method.Priority = 3;
                    _logger.Debug("CoreData", $"===方法{method.MethodInfo.Name}的优先级为3");
                }
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
            #region 设置监听方法
            foreach (var assembly in assemblys) {
                if (assembly.GetCustomAttribute(typeof(MyClassAttribute)) is not MyClassAttribute)
                    continue;
                // 获取内部方法
                var methods = assembly.GetMethods();
                foreach (var method in methods) {
                    foreach (var att in method.GetCustomAttributes()) {
                        if (att is not ListenAttribute a)
                            continue;
                        try {
                            if (ListenerTable.ContainsKey(a.Type)) {
                                var myInfo = new MyMethodInfo(method);
                                myInfo.Priority = 0;
                                ListenerTable[a.Type].Add(myInfo);
                            } else {
                                var myInfo = new MyMethodInfo(method);
                                myInfo.Priority = 0;
                                ListenerTable.Add(a.Type,new List<MyMethodInfo> { myInfo });
                            }
                            _logger.Debug("CoreData", "^^^导入监听服务成功");
                        }
                        catch (Exception ex) {
                            _logger.Error(ex, "^^^导入监听服务失败");
                        }
                    }
                }
            }
            #endregion
        }


        public async ValueTask LoadReturnWord() {
            ReturnWordTable returnWordTable = new();
            // 尝试获取官方回复词表
            if (_dataCenter.TryGetObj(Setting.FileConfig.ReturnWordTable, out object obj)) {
                if(obj is ReturnWordTable) {
                    returnWordTable = (ReturnWordTable)obj;
                }
            } else {
                if(_dataCenter.TryGetObj(Setting.FileConfig.ReturnWordBackup,out object backup)) {
                    if(backup is ReturnWordTable) {
                        returnWordTable = (ReturnWordTable)backup;
                    }
                }
            }
            // 设置remark
            returnWordTable.remark = returnWordTable.remark==null?"所有接口的回复词":returnWordTable.remark;
            // 填补本地回复词表内不存在的接口默认语句
            foreach (var name in FuncTable.Keys) {
                if(returnWordTable.table.ContainsKey(name)) {
                    continue;
                }
                returnWordTable.table.Add(name, "{0}");
            }
            // 存储回复词表
            _dataCenter.SaveFile(returnWordTable, Setting.FileConfig.ReturnWordTable , 3);
            _dataCenter.SaveFile(returnWordTable,Setting.FileConfig.ReturnWordBackup , 1);

        }
    }
}
