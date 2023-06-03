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
        /// 监听地址
        /// </summary>
        public string Location { get; set; } = "127.0.0.1";
        /// <summary>
        /// 监听接口
        /// </summary>
        public ushort Port { get; set; } = 8080;
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
    }

    /// <summary>
    /// 系统文档的位置
    /// </summary>
    public class FileConfig {
        public string RepositoryRoot { get; set; } = "\\Repository";
        public string ReplyTable { get; set; } = "\\Repository\\System";
        public string ReturnWordTable { get; set; } = "\\Repository\\System";
    }

    public class LoggerConfig {
        public string Location { get; set; } 
        public string Level { get; set; } = "normal";
    }
}
