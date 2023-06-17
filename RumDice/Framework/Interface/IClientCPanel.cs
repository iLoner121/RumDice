using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RumDice.Framework {
    public interface IClientCPanel {
        ValueTask SendMsg(string s, Post post);
        ValueTask SendMsg(Send send, Post post);
        ValueTask SendMsg(List<Send> sends, Post post);

        string UseMyService(string s);
        
        List<Send> SplitSend(Send send, Post post);
    }
}
