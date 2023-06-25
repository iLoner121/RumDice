using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace RumDice.Framework {
    // kook 卡片预设
    public enum KookMsgType {
        /// <summary>
        /// 文本格式
        /// </summary>
        Text = 0,
        /// <summary>
        /// 代码块格式
        /// </summary>
        Code = 1,
    }
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
        /// 消息去前缀
        /// </summary>
        /// <param name="msg">消息文本</param>
        /// <param name="prefix">指令前缀</param>
        /// <returns></returns>
        string GetMsgWithoutPrefix(string msg, string prefix);
        /// <summary>
        /// 制作回信包
        /// </summary>
        /// <param name="msg">回信包的消息内容</param>
        /// <param name="post">该回信包所回复的post</param>
        /// <param name="type">如果生成的是Kook回信包，那么该消息的显示类型（默认为行内代码）</param>
        /// <returns>根据post生成的对应私聊/群聊以及机器人类型的回信包</returns>
        Send MakeSend(string msg,Post post,KookMsgType type= KookMsgType.Code);
        /// <summary>
        /// 制作多条回信包
        /// </summary>
        /// <param name="msgs">多条回信包的消息内容</param>
        /// <param name="post">该回信包所回复的post</param>
        /// <param name="type">如果生成的是Kook回信包，那么该消息的显示类型（默认为行内代码）</param>
        /// <returns>根据post生成的对应私聊/群聊以及机器人类型的回信包</returns>
        List<Send> MakeSend(List<string> msgs, Post post,KookMsgType type = KookMsgType.Code);

        /// <summary>
        /// 复制回信包模式
        /// </summary>
        /// <param name="msg">新回信包的消息内容</param>
        /// <param name="send">原回信包</param>
        /// <returns>和原回信包模式相同的新对象</returns>
        Send MakeSend(string msg, Send send);
        /// <summary>
        /// 复制回信包模式
        /// </summary>
        /// <param name="msgs">新回信包的消息内容</param>
        /// <param name="send">原回信包</param>
        /// <returns>和原回信包模式相同的新对象</returns>
        List<Send> MakeSend(List<string> msgs, Send send);

        /// <summary>
        /// 生成输出语句
        /// </summary>
        /// <param name="fullname">该方法的全名:namespace.class.method (该方法必须被声明为回复接口)</param>
        /// <param name="paramList">参数列表（取决于回复json中的标记）</param>
        /// <param name="returnWordName">回复语句名字</param>
        /// <returns></returns>
        string GenerateMsg(string fullname, List<string> paramList, string returnWordName="default");
        /// <summary>
        /// 生成输出语句
        /// </summary>
        /// <param name="fullname">该方法的全名:namespace.class.method (该方法必须被声明为回复接口)</param>
        /// <param name="paramList">参数列表（取决于回复json中的标记）</param>
        /// <param name="returnWordName">回复语句名字“默认default"</param>
        /// <returns></returns>
        string GenerateMsg(string fullname, Dictionary<string,string> paramList,string returnWordName="default");

        /// <summary>
        /// 设置机器人类型
        /// </summary>
        /// <param name="post">收信包</param>
        /// <param name="send">需要被调整的发信包</param>
        /// <returns>调整后的发信包</returns>
        Send SetBotType(Post post,Send send);
        /// <summary>
        /// 设置机器人类型
        /// </summary>
        /// <param name="oldSend">另一个设置好的发信包</param>
        /// <param name="send">需要被调整的发信包</param>
        /// <returns>调整后的发信包</returns>
        Send SetBotType(Send oldSend,Send send);
    }
}
