using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RumDice.Framework {
    public class PokeNotice: BaseNotice {
        public long GroupID { get; set; }
        public long UserID { get; set; }
        public long TargetID { get; set; }
    }
}
