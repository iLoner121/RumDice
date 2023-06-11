using RumDice.Framework.Datatype;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RumDice.Framework {
    [MyStruct]
    public class StringListMap : AnyMap<List<string>>{
        public StringListMap(string remark = null) { 
            remark = remark ?? string.Empty;
        }
    }
}
