using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RumDice.Framework {
    /// <summary>
    /// 处理信息的辅助类
    /// </summary>
    [MyClass]
    public interface IMessageTool {
        /// <summary>
        /// 获取消息类型
        /// </summary>
        /// <param name="post"></param>
        /// <returns></returns>
        MessageType GetMsgType(Post post);
        /// <summary>
        /// 获取消息文本
        /// </summary>
        /// <param name="post"></param>
        /// <returns></returns>
        string GetTextMsg(Post post);
        /// <summary>
        /// 获取消息文本（去前缀）
        /// </summary>
        /// <param name="post"></param>
        /// <returns></returns>
        string GetMsgWithoutPrefix(Post post);
        /// <summary>
        /// 制作回信包
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="post"></param>
        /// <returns></returns>
        Send MakeSend(string msg,Post post);
        /// <summary>
        /// 制作多条回信包
        /// </summary>
        /// <param name="msgs"></param>
        /// <param name="post"></param>
        /// <returns></returns>
        List<Send> MakeSends(List<string> msgs, Post post);
    }
}
