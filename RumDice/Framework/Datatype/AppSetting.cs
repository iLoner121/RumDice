using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RumDice.Framework {
    public class AppSetting {
        public ServerConfig ServerConfig { get; set; } = new();
        public UserConfig UserConfig { get; set; }=new();
        public LoggerConfig LoggerConfig { get; set; } = new();
    }

    public class ServerConfig {
        /// <summary>
        /// 监听地址
        /// </summary>
        public string Location { get; set; } = "127.0.0.1";
        /// <summary>
        /// 监听接口
        /// </summary>
        public ushort Port { get; set; } = 8888;
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

    public class LoggerConfig {
        public string Location { get; set; } = Environment.CurrentDirectory+"\\Log";
        public string Level { get; set; } = "normal";
    }
}
