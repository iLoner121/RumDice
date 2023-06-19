using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RumDice.Framework {
    public interface IClientCPanel {
        void SendMsg(string s, Post post);
        void SendMsg(Send send, Post post);
        void SendMsg(List<Send> sends, Post post);

        ValueTask SendMsgAsync(string s, Post post);
        ValueTask SendMsgAsync(Send send, Post post);
        ValueTask SendMsgAsync(List<Send> sends, Post post);

        string UseMyService(string s);
        
        List<Send> SplitSend(Send send, Post post);

        void SendOperation(Send send);
        void SendOperation(List<Send> sends);

        ValueTask SendOperationAsync(Send send);
        ValueTask SendOperationAsync(List<Send> sends);
    }
}
