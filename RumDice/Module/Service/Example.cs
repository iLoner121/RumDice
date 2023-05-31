using RumDice.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RumDice.Module {
    public class Example : IExample {
        public void TestKeyWord1(Post post) {
            var GroupMessage = (GroupMessage)post;
            Console.WriteLine($"Test1 Match：{GroupMessage.Msg}");
        }

        public Send TestKeyWord2(Post post) {
            var GroupMessage = (GroupMessage)post;
            Console.WriteLine($"Test2 Match：{GroupMessage.Msg}");
            Send send = new Send();
            // send.GroupID = 11111111111;
            send.Msg = "你好啊！";
            return send;
        }

        public List<Send> TestKeyWord3(Post post) {
            var GroupMessage = (GroupMessage)post;
            Console.WriteLine($"Test3 Match：{GroupMessage.Msg}");
            Send send = new Send();
            // send.GroupID=11111111111;
            send.Msg = "hello！";
            return new List<Send> { send };
        }

        public string TestEcho(Post post) {
            var GroupMessage = (GroupMessage)post;
            string msg = GroupMessage.Msg;
            int index = msg.IndexOf('o');
            string echo = msg.Remove(0, index + 1);
            Console.WriteLine(echo);
            return echo;
        }
    }
}
