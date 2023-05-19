using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RumDice.Module {
    public class Example : IExample {
        public async ValueTask TestKeyWord(string k) {
            Console.WriteLine("Match!");
        }

        public async ValueTask TestKeyWord2(string k) {
            Console.WriteLine("Test2");
        }
    }
}
