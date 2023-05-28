using Microsoft.CodeAnalysis.CSharp.Syntax;
using RumDice.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace RumDice.Core {
    public interface IGlobalData {
        /// <summary>
        /// 初始设置
        /// </summary>
        public AppSetting Setting { get; set; }

        /// <summary>
        /// reply指令的对应表，只能为全匹配
        /// </summary>
        /// <returns>Dictionary(key:匹配词 , value:返回语句)</returns>
        Dictionary<string,string> ReplyTable { get; }


        /// <summary>
        /// 复杂指令匹配表，多关键词匹配，包含全匹配
        /// </summary>
        /// <returns>Dictionary(key:对应方法的MethodInfo , value:匹配词列表)</returns>
        Dictionary<List<KeyWordAttribute>,string> MatchTable { get; }


        /// <summary>
        /// 接口表（RumDice自带的所有回复接口）
        /// </summary>
        /// <returns>Dictionary(key:方法名 , value:方法的委托)</returns>
        Dictionary<string,MyMethodInfo> FuncTable { get; }

        /// <summary>
        /// 最小（高）优先级
        /// </summary>
        public int MinPriority { get; }
        /// <summary>
        /// 最大（低）优先级
        /// </summary>
        public int MaxPriority { get; }



        /// <summary>
        /// 初始化
        /// </summary>
        ValueTask Initialize();


        /// <summary>
        /// 扫描全局内内建方法
        /// </summary>
        ValueTask LoadInnerFunc();

        // TODO: 加载插件
        // TODO: 加载reply
        // TODO: 读取数据

        
    }
}
