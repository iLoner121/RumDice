using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RumDice.Framework;

namespace RumDice.Core {
    public interface IEventManager {
        /// <summary>
        /// 处理群聊消息
        /// </summary>
        /// <param name="post"></param>
        /// <returns></returns>
        ValueTask HandleGroupMessage(Post post);

        /// <summary>
        /// 处理私聊消息
        /// </summary>
        /// <param name="post"></param>
        /// <returns></returns>
        ValueTask HandlePrivateMessage(Post post);

    }
}
