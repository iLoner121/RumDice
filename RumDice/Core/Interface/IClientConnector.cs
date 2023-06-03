using EleCho.GoCqHttpSdk;
using RumDice.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RumDice.Core {
    public interface IClientConnector {
        CqWsSession Session { get; }

        ValueTask RunServer(string uri);
        ValueTask SendPrivateMsg(Send send);
        ValueTask SendGroupMsg(Send send);
    }
}
