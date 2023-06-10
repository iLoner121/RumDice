using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RumDice.Framework {
    public class GroupCardNotice : BaseNotice{
        public long GroupID { get; set; }
        public long UserID { get; set; }
        public string CardNew { get; set; }
        public string CardOld { get; set; }
    }
}
