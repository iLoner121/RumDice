using Microsoft.CodeAnalysis.CSharp.Syntax;
using RumDice.Framework;
using CSScriptLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Reflection.Emit;
using System.Dynamic;

namespace RumDice.Core{

    public class PluginSupporter : IPluginSupporter
    {
        public string CoreName = "PluginSupporter";
        public static IPluginSupporter? Instance { get; private set; }
        // public Dictionary<string, MethodInfo> PluginMethod{get; set;} = new();
        // public Dictionary<string, Object> PlugunObject{get; set;} = new();
        private IEvaluator evaluator{get; set;}
        private dynamic? script;
        private ICoreData? coreData;
        private string PluginLocal = "\\Module\\Plugin";

        public PluginSupporter(){
            evaluator = CSScript.Evaluator.ReferenceAssemblyByNamespace("RumDice.Core")
                                          .ReferenceAssemblyByNamespace("RumDice.FrameWork")
                                          .ReferenceAssemblyByNamespace("RumDice.Moudle");
        }

        public async ValueTask LoadPlugin(){
            await Task.Delay(0);
            IRumLogger rumLogger = RumLogger.Instance;
            coreData = CoreData.Instance;
            rumLogger.Debug(CoreName, "***开始加载插件");
            string[] allFiles = Directory.GetFiles(CoreData.Instance.RootDic+PluginLocal, "*.cs", SearchOption.AllDirectories);
            rumLogger.Debug(CoreName, $"***扫描到{allFiles.Length}个插件");
            int cnt = 0;
            foreach (string i in allFiles){
                if (coreData.FuncTable.ContainsKey(i))
                    continue;
                script = evaluator.LoadFile(i);
                if (Attribute.IsDefined(script.GetType(), typeof(MyClassAttribute)) == false){
                    continue;
                }
                rumLogger.Debug(CoreName, $"***已读取到插件类{script.name}");
                NewPlugin(script);
                cnt += 1;
            }
            rumLogger.Debug(CoreName, $"本次加载了{cnt}个插件类");
        }

        public void NewPlugin(dynamic script){
            var methods = script.GetMethods();
            foreach (var method in methods) {
                var atts = new List<KeyWordAttribute>();
                foreach (var att in method.GetCustomAttributes()) {
                    if(att is not KeyWordAttribute a) 
                        continue;
                    atts.Add(a);
                }
                if (atts.Count > 0){
                    try{
                        coreData.MatchTable.Add(atts, $"{script.FullName}.{method.Name}");
                        // coreData.KeyWordTable.Add(atts[0].KeyWord, $"{script.FullName}.{method.Name}");
                        coreData.FuncTable.Add($"{script.FullName}.{method.Name}", new MyMethodInfo(method, true));
                        RumLogger.Instance.Debug(CoreName, $"***已导入插件接口：{method.Name}");
                    }
                    catch (Exception ex){
                        RumLogger.Instance.Error(ex, "***导入插件接口失败：重复的关键词或方法名称");
                        if(coreData.MatchTable.ContainsKey(atts))
                            coreData.MatchTable.Remove(atts);
                        if (coreData.FuncTable.ContainsKey($"{script.FullName}.{method.Name}"))
                            coreData.FuncTable.Remove($"{script.FullName}.{method.Name}");
                        // if (coreData.KeyWordTable.ContainsKey(atts[0].KeyWord)){
                            // coreData.KeyWordTable.Remove(atts[0].KeyWord);
                    }
                }
            }
        }

        public ValueTask ReLoadPlugin()
        {
            throw new NotImplementedException();
        }
    }
}
