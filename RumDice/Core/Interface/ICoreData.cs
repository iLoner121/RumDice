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
    public interface ICoreData {
        /// <summary>
        /// 0开发模式 1发行模式（会影响文件读取的根目录）
        /// </summary>
        int Mode { get; }
        /// <summary>
        /// 0使用gocqhttp客户端（如果gocqhttp未开启RumDice会启动失败） 1使用测试程序
        /// </summary>
        int Test { get; }
        /// <summary>
        /// 获取应用所需文件的根目录
        /// </summary>
        public string RootDic { get; set; }
        /// <summary>
        /// 初始设置
        /// </summary>
        AppSetting Setting { get; set; }


        /// <summary>
        /// 复杂指令匹配表，多关键词匹配，包含全匹配
        /// </summary>
        Dictionary<List<KeyWordAttribute>,string> MatchTable { get; }

        /// <summary>
        /// 用于存取指令与对应接口，指令与设置相同
        /// </summary>
        Dictionary<string, string> KeyWordTable {get;}


        /// <summary>
        /// 接口表（RumDice自带的所有回复接口）
        /// </summary>
        Dictionary<string,MyMethodInfo> FuncTable { get; }

        /// <summary>
        /// 内置服务表
        /// </summary>
        Dictionary<string, MyMethodInfo> ServiceTable { get; }

        /// <summary>
        /// 服务监听表格
        /// </summary>
        Dictionary<AllType, List<MyMethodInfo>> ListenerTable { get; }

        


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

        /// <summary>
        /// 加载回复词库
        /// </summary>
        /// <returns></returns>
        ValueTask LoadReturnWord();

        /// <summary>
        /// 重新加载回复词库
        /// </summary>
        /// <returns></returns>
        ValueTask ReLoadRes(ReturnWordTable newRes, string requester);
    }
}
