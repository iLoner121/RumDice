using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace RumDice.Framework {
    public interface IRumLogger {
        void Debug(string caption, string logs = "", LogLevel logLevel = LogLevel.Debug, [CallerMemberName] string memberName = "", [CallerLineNumber] int lineNumber = 0, [CallerFilePath] string filePath = "");
        void Debug(Exception ex = null, string logs = "", LogLevel logLevel = LogLevel.Debug, [CallerMemberName] string memberName = "", [CallerLineNumber] int lineNumber = 0, [CallerFilePath] string filePath = "");
        void Info(string caption, string logs, LogLevel logLevel = LogLevel.Information, [CallerMemberName] string memberName = "", [CallerLineNumber] int lineNumber = 0, [CallerFilePath] string filePath = "");
        void Info(Exception ex, string logs, LogLevel logLevel = LogLevel.Information, [CallerMemberName] string memberName = "", [CallerLineNumber] int lineNumber = 0, [CallerFilePath] string filePath = ""); 
        void Warn(Exception ex, string logs, LogLevel logLevel = LogLevel.Warning, [CallerMemberName] string memberName = "", [CallerLineNumber] int lineNumber = 0, [CallerFilePath] string filePath = "");
        void Warn(string caption, string logs, LogLevel logLevel = LogLevel.Warning, [CallerMemberName] string memberName = "", [CallerLineNumber] int lineNumber = 0, [CallerFilePath] string filePath = "");
        void Error(Exception ex, string logs, LogLevel logLevel = LogLevel.Error, [CallerMemberName] string memberName = "", [CallerLineNumber] int lineNumber = 0, [CallerFilePath] string filePath = "");
        void Fatal(Exception ex, string logs, LogLevel logLevel = LogLevel.Fatal, [CallerMemberName] string memberName = "", [CallerLineNumber] int lineNumber = 0, [CallerFilePath] string filePath = "");
        void SetLogLevel(LogLevel logLevel);

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="appSetting"></param>
        /// <param name="RootDic"></param>
        void Initialize(AppSetting appSetting,string RootDic);

    }
}
