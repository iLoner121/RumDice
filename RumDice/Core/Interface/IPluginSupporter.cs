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
    public interface IPluginSupporter{

        // Dictionary<string, MethodInfo> PluginMethod{get; set;}
        // Dictionary<string, DynamicObject> PlugunObject{get; set;}

        /// <summary>
        /// 加载插件
        /// </summary>
        /// <returns></returns>
        ValueTask LoadPlugin();

        ValueTask ReLoadPlugin();

        // ValueTask Initialize();

    }
}
