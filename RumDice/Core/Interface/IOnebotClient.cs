using EleCho.GoCqHttpSdk;
using RumDice.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RumDice.Core {
    public interface IOnebotClient {
        void RecallMsg(Send send);

        void KickGroupMember(Send send);

        void BanGroupMember(Send send);

        void BanGroupAll(Send send);

        void CancelBanGroupMember(Send send);

        void CancelBanGroupAll(Send send);

        void SetGroupAdmin(Send send);

        void SetGroupCard(Send send);

        void SetGroupName(Send send);

        void LeaveGroup(Send send);

        void SetGroupTitle(Send send);

        void AcceptFriend(Send send);

        void AcceptGroup(Send send);

        void DeleteFriend(Send send);

        void DeleteNonFriend(Send send);

        void UploadGroupFile(Send send);

        List<CqGroup> GetGroupList();

        List<CqGroupMember> GetMemberList(long groupID);

        List<CqFriend> GetFriendList();

        List<CqFriend> GetNonFriendList();
    }
}
