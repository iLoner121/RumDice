using RumDice.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RumDice.Core {
    /// <summary>
    /// 信息发送管道
    /// </summary>
    public interface IMsgPipeline {

        ValueTask Initialize(int mode);

        /// <summary>
        /// 发送信息
        /// </summary>
        /// <param name="sends"></param>
        /// <returns></returns>
        void SendMsg(List<Send> sends);

        
        void RecvMsg(Post post,AllType type);

        void RecvPrivateMsg(Post post);
        void RecvGroupMsg(Post post);
        void RecvFriendRecallNotice(Post post);
        void RecvGroupRecallNotice(Post post);
        void RecvGroupIncreaseNotice(Post post);
        void RecvGroupDecreaseNotice(Post post);
        void RecvGroupAdminNotice(Post post);
        void RecvGroupBanNotice(Post post);
        void RecvFriendAddNotice(Post post);
        void RecvGroupPokeNotice(Post post);
        void RecvHonorNotice(Post post);
        void RecvTitleNotice(Post post);
        void RecvGroupCardNotice(Post post);
        void RecvFriendRequest(Post post);
        void RecvGroupRequest(Post post);
    }
}
