using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RumDice.Framework {
    public class BaseRequest :Post {
        public RequestType RequestType { get; set; }
    }
}
