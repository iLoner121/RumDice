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
        //[KeyWord(".r", isPrefix: true)]
        void TestKeyWord1(Post post);

        /// <summary>
        /// 测试接口（优先级、返回单条消息包）
        /// </summary>
        /// <param name="post"></param>
        /// <returns></returns>
        //[KeyWord(".ra",isPrefix:true)]
        //[Priority(1)]
        Send TestKeyWord2(Post post);

        /// <summary>
        /// 测试接口（返回多条信息包）
        /// </summary>
        /// <param name="post"></param>
        /// <returns></returns>
        [KeyWord(".jrrp", isPrefix: true)]
        List<Send> TestKeyWord3(Post post);

        /// <summary>
        /// 测试接口（返回string）
        /// </summary>
        /// <param name="post"></param>
        /// <returns></returns>
        [KeyWord("echo Echo ECHO .echo .Echo .ECHO",isPrefix:true)]
        [IsPrivate]
        string TestEcho(Post post);

    }
}
