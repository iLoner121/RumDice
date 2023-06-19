using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RumDice.Framework {
    public enum OneBotAction {
        /// <summary>
        /// 无（说明此包为群聊或私聊）
        /// </summary>
        None,
        /// <summary>
        /// 撤回信息
        /// </summary>
        RecallMsg,
        /// <summary>
        /// 踢出群成员
        /// </summary>
        KickGroupMember,
        /// <summary>
        /// 禁言群成员
        /// </summary>
        BanGroupMember,
        /// <summary>
        /// 全体禁言
        /// </summary>
        BanGroupAll,
        /// <summary>
        /// 解除群成员禁言
        /// </summary>
        CancelBanGroupMember,
        /// <summary>
        /// 解除全体禁言
        /// </summary>
        CancelBanGroupAll,
        /// <summary>
        /// 设置群管理员
        /// </summary>
        SetGroupAdmin,
        /// <summary>
        /// 设置群名片
        /// </summary>
        SetGroupCard,
        /// <summary>
        /// 设置群名
        /// </summary>
        SetGroupName,
        /// <summary>
        /// 离开群聊
        /// </summary>
        LeaveGroup,
        /// <summary>
        /// 设置专属头衔
        /// </summary>
        SetGroupTitle,
        /// <summary>
        /// 接受好友申请
        /// </summary>
        AcceptFriend,
        /// <summary>
        /// 接受加群申请
        /// </summary>
        AcceptGroup,
        /// <summary>
        /// 删除好友
        /// </summary>
        DeleteFriend,
        /// <summary>
        /// 删除单项好友
        /// </summary>
        DeleteNonFriend,
        /// <summary>
        /// 上传群文件
        /// </summary>
        UploadGroupFile,
    }
}
