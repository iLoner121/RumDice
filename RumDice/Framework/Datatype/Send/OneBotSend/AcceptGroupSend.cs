using EleCho.GoCqHttpSdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RumDice.Framework {
    /// <summary>
    /// 通过加群请求
    /// </summary>
    public class AcceptGroupSend :OneBotSend{
        public String Flag { get; set; }
        public CqGroupRequestType RequestType { get; set; }
        /// <summary>
        /// 通过加群请求发信包
        /// </summary>
        /// <param name="flag">群聊申请标识码（在好友请求包中获得）</param>
        /// <param name="requestType">加群请求类型（加群申请/群聊邀请）</param>
        public AcceptGroupSend(string flag,CqGroupRequestType requestType) {
            Action = OneBotAction.AcceptGroup;
            Flag = flag;
            RequestType = requestType;
        }
    }
}
