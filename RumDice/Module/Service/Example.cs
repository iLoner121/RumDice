using RumDice.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RumDice.Module {
    public class Example : IExample {
        /// <summary>
        /// 实例服务Echo
        /// </summary>
        /// <param name="post">必须声明为Post类型的参数</param>
        /// <returns>可以返回void, string, Send, List\<Send\>作为发信内容</returns>
        public string TestEcho(Post post) {
            // 用显式将post转换为各种Message格式（详见文档内的各种内置格式的说明）
            var baseMessage = (BaseMessage)post;

            // 处理消息
            string msg = baseMessage.Msg;
            int index = msg.IndexOf('o');
            string echo = msg.Remove(0, index + 1);

            /* 
             *使用MessageTool的GenerateMessage方法生成消息
             *这种方式可以让你能够便捷的在json文件中自定义回复语句的内容
             *当然你也可以绕过这一步，直接返回自己想要的字符串
             *但是我十分建议你用我提供这个方法
            */
            echo = new MessageTool().GenerateMessage("RumDice.Module.IExample.TestEcho", new List<string> { echo });


            // 如果我想要调用数据库，比如现在我想要取出系统的回复词表
            var returnWords = (ReturnWordTable)DataCenter.Instance.GetObj("\\System\\Text\\ReturnWordTable.json");
            // 或者我也可以直接访问系统设置查看各种预制的文件位置
            returnWords = (ReturnWordTable)DataCenter.Instance.GetObj(DataCenter.Instance.AppSetting.FileConfig.ReturnWordTable);


            // 当需要日志服务的时候使用此方式获取一个RumLogger实例
            // 请注意RumLogger为一个单例，请不要new出它的新实例，它会缺少正确的初始化数据
            IRumLogger logger = RumLogger.Instance;
            string word = returnWords.table["RumDice.Module.IExample.TestEcho"];
            logger.Debug("echo", $"echo测试程序默认的返回词是：{word}");

            DataCenter.Instance.GetObj("xx");


            // 返回消息
            return echo;
        }

        public void TestKeyWord1(Post post) {
            Console.WriteLine("testkeyword1已被触发");
        }

        public Send TestKeyWord2(Post post) {
            Send send = new Send();
            send.Msg = new MessageTool().GenerateMessage("RumDice.Module.IExample.TestKeyWord2", new List<string> { "TestKeyWord2的参数" });
            return send; 
        }

        public List<Send> TestKeyWord3(Post post) {
            Send send = new Send();
            // send.GroupID=11111111111;
            send.Msg = new MessageTool().GenerateMessage("RumDice.Module.IExample.TestKeyWord2", new List<string> { "TestKeyWord3的参数" });
            return new List<Send> { send };
        }

        public string EchoTest(string s) {
            return "很高兴程序运行一切正常";
        }
    }
}
