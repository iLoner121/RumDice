using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RumDice.Framework {
    [MyStruct]
    public class AppSetting {
        public ServerConfig ServerConfig { get; set; } = new();
        public UserConfig UserConfig { get; set; }=new();
        public LoggerConfig LoggerConfig { get; set; } = new();
        public FileConfig FileConfig { get; set; }=new();
    }

    public class ServerConfig {
        /// <summary>
        /// 是否启动qq机器人
        /// </summary>
        public bool QQbot { get; set; } = true;
        /// <summary>
        /// 是否启动KOOK机器人
        /// </summary>
        public bool KOOKbot { get; set; } = true;
        /// <summary>
        /// gocqhttp监听地址
        /// </summary>
        public string Location { get; set; } = "127.0.0.1";
        /// <summary>
        /// gocqhttp监听接口
        /// </summary>
        public ushort Port { get; set; } = 8080;
        /// <summary>
        /// kook机器人token
        /// </summary>
        public string KookToken { get; set; } = "";
    }

    public class UserConfig {
        /// <summary>
        /// 是否接收信息
        /// </summary>
        public bool IsListen { get; set; } = true;
        /// <summary>
        /// 是否使用插件
        /// </summary>
        public bool IsUsingPlugin { get; set; } = true;
        /// <summary>
        /// 是否被锁定（能否添加好友）
        /// </summary>
        public bool IsLocked { get; set; } = true;
        /// <summary>
        /// 发送信息的最大时间间隔
        /// </summary>
        public int MaxDuration { get; set; } = 400;
        /// <summary>
        /// 发送信息的最小时间间隔
        /// </summary>
        public int MinDuration { get; set; } = 200;
        /// <summary>
        /// 接收队列的检查器唤醒时间间隔
        /// </summary>
        public int RecvWakeDuration { get; set; } = 1000;
        /// <summary>
        /// 发信队列的检查器唤醒时间间隔
        /// </summary>
        public int SendWakeDuration { get; set; } = 1000;
        /// <summary>
        /// 线程池最大数量
        /// </summary>
        public int MaxThreadCount { get; set; } = 16;
    }

    /// <summary>
    /// 系统文档的位置
    /// </summary>
    public class FileConfig {
        /// <summary>
        /// 储存器根目录
        /// </summary>
        public string RepositoryRoot { get; set; } = "\\File";  // 项目根目录
        /// <summary>
        /// 系统回复词表格
        /// </summary>
        public string ReturnWordTable { get; set; } = "\\System\\Text\\ReturnWordTable.json";
        public string ReturnWordBackup { get; set; } = "\\System\\Text\\ReturnWordBackup.json";
        public string DefaultRes {get; set;} = "\\System\\Text\\DefaultRes.json";
    }

    public class LoggerConfig {
        /// <summary>
        /// Log默认存储文件夹（在根目录下，也就是和AppSetting的位置同级）
        /// </summary>
        public string Location { get; set; } = "\\File\\Log";
        /// <summary>
        /// 最低处理等级（低于此等级的log将不会被显示和处理）
        /// </summary>
        public LogLevel Level { get; set; } = LogLevel.Debug;
        /// <summary>
        /// 最低写入等级（低于此等级的log将不会被写入本地的log文件内；此等级不得低于Level）
        /// </summary>
        public LogLevel OutputLevel { get; set; } = LogLevel.Information;
        /// <summary>
        /// 是否在控制台显示log细节（设为false则只有一行输出，内容仅包括一条文字描述）
        /// </summary>
        public bool ShowDetail { get; set; } = false;
        /// <summary>
        /// 是否在本地记录log细节（设为false则只有一行输出，内容仅包括一条文字描述）（debug和info永久只能写入简单记录）
        /// </summary>
        public bool OutputDetail { get; set; } = true;
    }
}
