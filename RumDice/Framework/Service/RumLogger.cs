using RumDice.Core;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace RumDice.Framework {
    public class RumLogger : IRumLogger {

        string _path;
        LogLevel _logLevel;
        LogLevel _outputLevel;
        bool _showDetail;
        bool _outputDetail;
        object _lock = new object();
        ConsoleColor _consoleColor = Console.BackgroundColor;
        CultureInfo _cultureInfo = CultureInfo.CurrentCulture;

        public static IRumLogger Instance { get; private set; }

        public RumLogger() {
            Instance = this;
        }


        public IRumLogger ChangeColor(ConsoleColor fColor, ConsoleColor bColor) {
            ChangeConsoleColor(fColor, bColor);
            return this;
        }

        /// <summary>
        /// 改变颜色
        /// </summary>
        /// <param name="fColor"></param>
        /// <param name="bColor"></param>
        private static void ChangeConsoleColor(ConsoleColor fColor, ConsoleColor bColor) {
            Console.ForegroundColor = fColor;
            Console.BackgroundColor = bColor;
        }

        /// <summary>
        /// 写入log到文件夹
        /// </summary>
        /// <param name="ex"></param>
        /// <param name="logs"></param>
        /// <param name="logLevel"></param>
        /// <param name="memberName"></param>
        /// <param name="lineNumber"></param>
        /// <param name="filePath"></param>
        private void WriteLogToFile(Exception ex, string logs, LogLevel logLevel = LogLevel.Error, [CallerMemberName] string memberName = "", [CallerLineNumber] int lineNumber = 0, [CallerFilePath] string filePath = "") {
            var logPath = $"{_path}";
            if (Directory.Exists(logPath) is false)
                Directory.CreateDirectory(logPath);
            logPath = Path.Combine(logPath, $"{DateTime.Now:yyyyMMdd}.log");
            if (File.Exists(logPath) is false) {
                using var streamLog = new FileStream(logPath, FileMode.CreateNew);
            }
            using var stream = new FileStream(logPath, FileMode.Append);
            var writer = new StreamWriter(stream);
            if (_outputDetail&&logLevel>LogLevel.Information) {
                writer.Write($"时间:[{DateTime.Now}]\t\n日志等级:[{logLevel}]\t\n异常:{ex}\t\n日志:{logs}\t\n调用方法:[{memberName}]\t\n行号:[{lineNumber}]\t\n文件路径:[{filePath}]\r\r");
            } else {
                string wu = ex==null ? "无" : ex.ToString();
                writer.Write($"[{DateTime.Now}]-[{logLevel}] -- 日志:{logs} || 异常:{wu}\r\r");
            }
            writer.Flush();
            writer.Close();
        }



        public void Debug(Exception ex = null, string logs = "", LogLevel logLevel = LogLevel.Debug, [CallerMemberName] string memberName = "", [CallerLineNumber] int lineNumber = 0, [CallerFilePath] string filePath = "") {
            if (_logLevel > logLevel) return;
            lock (_lock) {
                ChangeConsoleColor(ConsoleColor.White, _consoleColor);
                Console.Write($@"[ {DateTime.Now.ToString(_cultureInfo)} ] | ");
                ChangeConsoleColor(ConsoleColor.Black, ConsoleColor.Gray);
                Console.Write(@" [Debug] ");
                ChangeConsoleColor(ConsoleColor.White, _consoleColor);
                Console.WriteLine($"[{ex?.GetType() ?? typeof(IRumLogger)}]{logs}");
                if (_showDetail) {
                    Console.WriteLine($"[调用方法]{memberName}");
                    Console.WriteLine($"[行号]{lineNumber}");
                    Console.WriteLine($"[路径]{filePath}\r\n");
                }
                if (_outputLevel <= logLevel) {
                    WriteLogToFile(ex, logs, logLevel, memberName, lineNumber, filePath);
                }
            }
        }
        public void Debug(string caption="", string logs = "", LogLevel logLevel = LogLevel.Debug, [CallerMemberName] string memberName = "", [CallerLineNumber] int lineNumber = 0, [CallerFilePath] string filePath = "") {
            if (_logLevel > logLevel) return;
            lock (_lock) {
                ChangeConsoleColor(ConsoleColor.White, _consoleColor);
                Console.Write($@"[ {DateTime.Now.ToString(_cultureInfo)} ] | ");
                ChangeConsoleColor(ConsoleColor.Black, ConsoleColor.Gray);
                Console.Write(@" [Debug] ");
                ChangeConsoleColor(ConsoleColor.White, _consoleColor);
                Console.WriteLine($"[{caption}]{logs}");
                if (_showDetail) {
                    Console.WriteLine($"[调用方法]{memberName}");
                    Console.WriteLine($"[行号]{lineNumber}");
                    Console.WriteLine($"[路径]{filePath}\r\n");
                }
                if (_outputLevel <= logLevel) {
                    WriteLogToFile(null, logs, logLevel, memberName, lineNumber, filePath);
                }
            }
        }

        public void Error(Exception ex, string logs, LogLevel logLevel = LogLevel.Error, [CallerMemberName] string memberName = "", [CallerLineNumber] int lineNumber = 0, [CallerFilePath] string filePath = "") {
            if (_logLevel > logLevel) return;
            lock (_lock) {
                ChangeConsoleColor(ConsoleColor.DarkRed, _consoleColor);
                Console.Write($@"[ {DateTime.Now.ToString(_cultureInfo)} ] | ");
                ChangeConsoleColor(ConsoleColor.Black, ConsoleColor.DarkRed);
                Console.Write(@" [Error] ");
                ChangeConsoleColor(ConsoleColor.DarkRed, _consoleColor);
                Console.WriteLine($"[{ex.GetType()}]{logs}");
                if (_showDetail) {
                    Console.WriteLine($"[调用方法]{memberName}");
                    Console.WriteLine($"[行号]{lineNumber}");
                    Console.WriteLine($"[路径]{filePath}\r\n");
                }
                ChangeConsoleColor(ConsoleColor.White, _consoleColor);
                if (_outputLevel <= logLevel) {
                    WriteLogToFile(ex, logs, logLevel, memberName, lineNumber, filePath);
                }
            }
        }

        public void Fatal(Exception ex, string logs, LogLevel logLevel = LogLevel.Fatal, [CallerMemberName] string memberName = "", [CallerLineNumber] int lineNumber = 0, [CallerFilePath] string filePath = "") {
            if (_logLevel > logLevel) return;
            lock (_lock) {
                ChangeConsoleColor(ConsoleColor.Cyan, _consoleColor);
                Console.Write($@"[ {DateTime.Now.ToString(_cultureInfo)} ] | ");
                ChangeConsoleColor(ConsoleColor.Black, ConsoleColor.Cyan);
                Console.Write(@" [Fatal] ");
                ChangeConsoleColor(ConsoleColor.Cyan, _consoleColor);
                Console.WriteLine($"[{ex.GetType()}]{logs}");
                if (_showDetail) {
                    Console.WriteLine($"[调用方法]{memberName}");
                    Console.WriteLine($"[行号]{lineNumber}");
                    Console.WriteLine($"[路径]{filePath}\r\n");
                }
                ChangeConsoleColor(ConsoleColor.White, _consoleColor);
                if (_outputLevel <= logLevel) {
                    WriteLogToFile(ex, logs, logLevel, memberName, lineNumber, filePath);
                }
            }
        }

        public void Info(string caption, string logs, LogLevel logLevel = LogLevel.Information, [CallerMemberName] string memberName = "", [CallerLineNumber] int lineNumber = 0, [CallerFilePath] string filePath = "") {
            if (_logLevel > logLevel) return;
            lock (_lock) {
                ChangeConsoleColor(ConsoleColor.White, _consoleColor);
                Console.Write($@"[ {DateTime.Now.ToString(_cultureInfo)} ] | ");
                ChangeConsoleColor(ConsoleColor.Black, ConsoleColor.DarkGreen);
                Console.Write(@" [Info] ");
                ChangeConsoleColor(ConsoleColor.White, _consoleColor);
                Console.WriteLine($"[{caption}]{logs}");
                if (_showDetail) {
                    Console.WriteLine($"[调用方法]{memberName}");
                    Console.WriteLine($"[行号]{lineNumber}");
                    Console.WriteLine($"[路径]{filePath}\r\n");
                }
                if (_outputLevel <= logLevel) {
                    WriteLogToFile(null, logs, logLevel, memberName, lineNumber, filePath);
                }
            }
        }

        public void Info(Exception ex, string logs, LogLevel logLevel = LogLevel.Information, [CallerMemberName] string memberName = "", [CallerLineNumber] int lineNumber = 0, [CallerFilePath] string filePath = "") {
            if (_logLevel > logLevel) return;
            lock (_lock) {
                ChangeConsoleColor(ConsoleColor.White, _consoleColor);
                Console.Write($@"[ {DateTime.Now.ToString(_cultureInfo)} ] | ");
                ChangeConsoleColor(ConsoleColor.Black, ConsoleColor.DarkGreen);
                Console.Write(@" [Info] ");
                ChangeConsoleColor(ConsoleColor.White, _consoleColor);
                Console.WriteLine($"[{ex.GetType()}]{logs}");
                if(_showDetail) {
                    Console.WriteLine($"[调用方法]{memberName}");
                    Console.WriteLine($"[行号]{lineNumber}");
                    Console.WriteLine($"[路径]{filePath}\r\n");
                }
                if (_outputLevel <= logLevel) {
                    WriteLogToFile(ex, logs, logLevel, memberName, lineNumber, filePath);
                }
            }
        }


        public void SetLogLevel(LogLevel logLevel) {
            if (Enum.IsDefined(typeof(LogLevel), logLevel))
                _logLevel = logLevel;
        }

        public void Warn(Exception ex, string logs, LogLevel logLevel = LogLevel.Warning, [CallerMemberName] string memberName = "", [CallerLineNumber] int lineNumber = 0, [CallerFilePath] string filePath = "") {
            if (_logLevel > logLevel) return;
            lock (_lock) {
                ChangeConsoleColor(ConsoleColor.DarkYellow, _consoleColor);
                Console.Write($@"[ {DateTime.Now.ToString(_cultureInfo)} ] | ");
                ChangeConsoleColor(ConsoleColor.Black, ConsoleColor.DarkYellow);
                Console.Write(@" [Warn] ");
                ChangeConsoleColor(ConsoleColor.DarkYellow, _consoleColor);
                Console.WriteLine($"[{ex.GetType()}]{logs}");
                if( _showDetail ) {
                    Console.WriteLine($"[调用方法]{memberName}");
                    Console.WriteLine($"[行号]{lineNumber}");
                    Console.WriteLine($"[路径]{filePath}\r\n");
                }
                ChangeConsoleColor(ConsoleColor.White, _consoleColor);
                if (_outputLevel <= logLevel) {
                    WriteLogToFile(ex, logs, logLevel, memberName, lineNumber, filePath);
                }
            }
        }

        public void Warn(string caption, string logs, LogLevel logLevel = LogLevel.Warning, [CallerMemberName] string memberName = "", [CallerLineNumber] int lineNumber = 0, [CallerFilePath] string filePath = "") {
            if (_logLevel > logLevel) return;
            lock (_lock) {
                ChangeConsoleColor(ConsoleColor.DarkYellow, _consoleColor);
                Console.Write($@"[ {DateTime.Now.ToString(_cultureInfo)} ] | ");
                ChangeConsoleColor(ConsoleColor.Black, ConsoleColor.DarkYellow);
                Console.Write(@" [Warn] ");
                ChangeConsoleColor(ConsoleColor.DarkYellow, _consoleColor);
                Console.WriteLine($"[{caption}]{logs}");
                if (_showDetail) {
                    Console.WriteLine($"[调用方法]{memberName}");
                    Console.WriteLine($"[行号]{lineNumber}");
                    Console.WriteLine($"[路径]{filePath}\r\n");
                }
                ChangeConsoleColor(ConsoleColor.White, _consoleColor);
                if (_outputLevel <= logLevel) {
                    WriteLogToFile(null, logs, logLevel, memberName, lineNumber, filePath);
                }
            }
        }

        public void Initialize(AppSetting appSetting, string RootDic) {
            _path = RootDic + appSetting.LoggerConfig.Location;
            _logLevel = appSetting.LoggerConfig.Level;
            _outputLevel = appSetting.LoggerConfig.OutputLevel;
            _showDetail = appSetting.LoggerConfig.ShowDetail;
            _outputDetail = appSetting.LoggerConfig.OutputDetail;
        }
    }
}
