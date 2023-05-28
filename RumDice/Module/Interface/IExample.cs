using RumDice.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RumDice.Module {
    [MyClass]
    public interface IExample {

        [KeyWord(".r", isPrefix: true)]
        void TestKeyWord(Post post);


        [KeyWord(".ra",isPrefix:true)]
        [Priority(1)]
        void TestKeyWord2(Post post);



    }
}
