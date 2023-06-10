using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RumDice.Framework {
    public class GroupRequest : BaseRequest {
        public long GroupID { get; set; }
        public long UserID { get; set; }
        public string Comment { get; set; }
        public string Flag { get; set; }
        public string SubType { get; set; }
    }
}
