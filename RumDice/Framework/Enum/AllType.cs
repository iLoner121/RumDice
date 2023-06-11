using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RumDice.Framework {
    public enum AllType : int{
        Post = 0,  
        Msg = 1,  
        PrivateMsg = 2,
        GroupMsg = 3,
        FriendRecall = 5,
        GroupRecall = 6,
        GroupIncrease = 7,
        GroupDecrease = 8,
        GroupAdmin = 9,
        GroupBan = 11,
        FriendAdd = 12,
        GroupPoke = 13,
        Honor =15,
        Title = 16,
        GroupCard = 17,
        FriendRequest = 18,
        GroupRequest = 19,
        None=20,


        Start=100,  // 系统启动后
        Send =200,  // 发送消息后
    }
}
