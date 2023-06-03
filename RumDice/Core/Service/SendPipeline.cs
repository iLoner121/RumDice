using RumDice.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RumDice.Core {
    public class SendPipeline : ISendPipeline {
        Queue<Send> quene = new();
        int _minDuration;
        int _maxDuration;
        public bool SendMsg(List<Send> sends) {
            return true;
        }
    }
}
