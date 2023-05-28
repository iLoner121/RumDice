using RumDice.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RumDice.Module {
    public class Example : IExample {
        public async void TestKeyWord(Post post) {
            var GroupMessage = (GroupMessage)post;
            Console.WriteLine($"Match!：{GroupMessage.Msg}");
        }

        public async void TestKeyWord2(Post post) {
            Console.WriteLine("Test2");
        }
    }
}
