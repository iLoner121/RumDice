using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace RumDice.Framework {
    /// <summary>
    /// 处理信息的辅助类
    /// </summary>
    [MyClass]
    public interface IMsgTool {
        /// <summary>
        /// 获取消息类型
        /// </summary>
        /// <param name="post"></param>
        /// <returns></returns>
        MsgType GetMsgType(Post post);
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
        /// <param name="prefix">前缀指令（不要在前面带.和。）</param>
        /// <returns></returns>
        string GetMsgWithoutPrefix(Post post,string prefix);
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
        List<Send> MakeSend(List<string> msgs, Post post);

        /// <summary>
        /// 生成输出语句
        /// </summary>
        /// <param name="fullname">该方法的全名:namespace.class.method (该方法必须被声明为回复接口)</param>
        /// <param name="paramList">参数列表（取决于回复json中的标记）</param>
        /// <returns></returns>
        string GenerateMsg(string fullname, List<string> paramList);
        /// <summary>
        /// 生成输出语句
        /// </summary>
        /// <param name="fullname">该方法的全名:namespace.class.method (该方法必须被声明为回复接口)</param>
        /// <param name="paramList">参数列表（取决于回复json中的标记）</param>
        /// <returns></returns>
        string GenerateMsg(string fullname, Dictionary<string,string> paramList);
    }
}
