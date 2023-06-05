using RumDice.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RumDice.Module {
    [MyClass]
    public interface IExample {
        /// <summary>
        /// 测试接口（无返回值）
        /// </summary>
        /// <param name="post"></param>
        [KeyWord(".prefixt", isPrefix: true)]
        void TestKeyWord1(Post post);

        /// <summary>
        /// 测试接口（优先级、返回单条消息包）
        /// </summary>
        /// <param name="post"></param>
        /// <returns></returns>
        [KeyWord(".prefixtest",isPrefix:true)]
        [Priority(1)]
        Send TestKeyWord2(Post post);

        /// <summary>
        /// 测试接口（返回多条信息包）
        /// </summary>
        /// <param name="post"></param>
        /// <returns></returns>
        [Reply("replytest")]
        List<Send> TestKeyWord3(Post post);

        /// <summary>
        /// 测试接口（返回string）
        /// </summary>
        /// <param name="post"></param>
        /// <returns></returns>
        [PrefixMatch("echo")] 
        [IsPrivate]
        string TestEcho(Post post);

        [MyService("echotest")]
        string EchoTest(string s);

    }
}
