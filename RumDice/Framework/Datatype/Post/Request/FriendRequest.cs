using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RumDice.Framework {
    public class FriendRequest : BaseRequest{
        public long UserID { get; set; }
        public string Commnet { get; set; }
        public string Flag { get; set; }
    }
}
