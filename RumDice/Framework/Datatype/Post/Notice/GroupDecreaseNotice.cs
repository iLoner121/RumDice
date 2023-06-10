using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RumDice.Framework {
    public class GroupDecreaseNotice : BaseNotice{
        public long GroupID { get; set; }
        public string SubType { get; set; }
        public long OperatorID { get; set; }
        public long UserID { get; set; }
    }
}
