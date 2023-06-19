using EleCho.GoCqHttpSdk;
using RumDice.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RumDice.Core {
    public interface IBaseClient {
        void RunServer(string uri);
        ValueTask RunServerAsync(string uri);
        void SendPrivateMsg(Send send);
        void SendGroupMsg(Send send);

    }
}
