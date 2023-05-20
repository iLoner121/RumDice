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
        /// 测试符合匹配。唯一匹配词为hellopkpp
        /// </summary>
        /// <param name="k"></param>
        /// <returns></returns>
        [KeyWord("hello fucku",IsPrefix =true)]
        [KeyWord("pp kk gg tt dhafiefia",IsSuffix=true)]
        [KeyWord("9 d 12 p")]
        [KeyWord("hellopkpp",IsFullMatch =true)]
        [KeyWord("hell kpp",isPrefix:true,isSuffix:true)]
        ValueTask TestKeyWord(string k);

        [KeyWord("echo",isPrefix:true)]
        ValueTask TestKeyWord2(string k);
        
    }
}
