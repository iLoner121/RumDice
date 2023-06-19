using CSScripting;
using EleCho.GoCqHttpSdk;
using EleCho.GoCqHttpSdk.Action;
using Microsoft.Extensions.DependencyInjection;
using RumDice.Framework;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RumDice.Core {
    public class QQClientConnector : IBaseClient,IOnebotClient {
        CqWsSession _session { get; set; } 
        readonly IServiceProvider _serviceProvider;
        readonly IRumLogger _logger;

        public QQClientConnector(IServiceProvider serviceProvider,IRumLogger rumLogger) { 
            _serviceProvider = serviceProvider;
            _logger = rumLogger;
        }

        public void SendPrivateMsg(Send send) {
            _session.SendPrivateMessage(send.UserID, new EleCho.GoCqHttpSdk.Message.CqMessage(send.Msg));
            return;
        }
        public void SendGroupMsg(Send send) {
            _session.SendGroupMessage(send.GroupID, new EleCho.GoCqHttpSdk.Message.CqMessage(send.Msg));
            return;
        }

        public void RecallMsg(Send send) {
            if (send is not RecallMsgSend s)
                return;
            _session.RecallMessage(s.MessageID);
            return;
        }

        public void KickGroupMember(Send send) {
            if (send is not KickGroupMemberSend s)
                return;
            _session.KickGroupMember(s.GroupID, s.UserID, s.RejectRequest);
            return;
        }

        public void BanGroupMember(Send send) {
            if (send is not BanGroupMemberSend s)
                return;
            _session.BanGroupMember(s.GroupID, s.UserID, s.Duration);
            return;
        }

        public void BanGroupAll(Send send) {
            if (send is not BanGroupAllSend s)
                return;
            _session.BanGroupAllMembers(s.GroupID);
            return;
        }

        public void CancelBanGroupMember(Send send) {
            if(send is not CancelBanGroupMemberSend s)
                return;
            _session.CancelBanGroupMember(s.GroupID, s.UserID);
            return;
        }

        public void CancelBanGroupAll(Send send) {
            if (send is not CancelBanGroupAllSend s)
                return;
            _session.CancelBanGroupAllMembers(s.GroupID);
        }

        public void SetGroupAdmin(Send send) {
            if(send is not SetGroupAdminSend s)
                return;
            _session.SetGroupAdministrator(s.GroupID, s.UserID, s.Enable);
            return;
        }

        public void SetGroupCard(Send send) {
            if(send is not SetGroupCardSend s) 
                return;
            _session.SetGroupNickname(s.GroupID, s.UserID, s.CardName);
            return;
        }

        public void SetGroupName(Send send) {
            if (send is not SetGroupNameSend s)
                return;
            _session.SetGroupName(s.GroupID, s.GroupName);
            return;
        }

        public void LeaveGroup(Send send) {
            if (send is not LeaveGroupSend s)
                return;
            _session.LeaveGroup(s.GroupID);
            return;
        }

        public void SetGroupTitle(Send send) {
            if (send is not SetGroupTitleSend s)
                return;
            _session.SetGroupSpecialTitle(s.GroupID,s.UserID,s.Title);
            return;
        }

        public void AcceptFriend(Send send) {
            if (send is not AcceptFriendSend s)
                return;
            _session.ApproveFriendRequest(s.Flag,s.Remark);
            return;
        }

        public void AcceptGroup(Send send) {
            if (send is not AcceptGroupSend s)
                return;
            _session.ApproveGroupRequest(s.Flag, s.RequestType);
            return;
        }

        public void DeleteFriend(Send send) {
            if (send is not DeleteFriendSend s)
                return;
            _session.DeleteFriend(s.UserID);
            return;
        }

        public void DeleteNonFriend(Send send) {
            if (send is not DeleteNonFriendSend s)
                return;
            _session.DeleteUnidirectionalFriend(s.UserID);
            return;
        }

        public void UploadGroupFile(Send send) {
            if(send is not UploadGroupFileSend s)
                return;
            _session.UploadGroupFile(s.GroupID, s.FilePath, s.FileName);
            return;
        }

        public List<CqGroup> GetGroupList() {
            var gl = _session.GetGroupList();
            List<CqGroup> res = new();
            if (gl == null)
                return res;
            res = gl.Groups.ToList();
            return res;
        }

        public List<CqGroupMember> GetMemberList(long groupID) {
            var ml = _session.GetGroupMemberList(groupID);
            List<CqGroupMember> res = new();
            if(ml == null) return res;
            res = ml.Members.ToList();
            return res;
        }

        public List<CqFriend> GetFriendList() {
            var fl = _session.GetFriendList();
            List<CqFriend> res = new();
            if(fl == null) return res;
            res = fl.Friends.ToList();
            return res;
        }

        public List<CqFriend> GetNonFriendList() {
            var nfl = _session.GetUnidirectionalFriendList();
            List<CqFriend> res = new();
            if(nfl == null) return res;
            res = nfl.Friends.ToList();
            return res;
        }

        public void RunServer(string uri) {
            RunServerAsync(uri).AsTask().Wait();
        }

        public async ValueTask RunServerAsync(string uri) {
            _session = new CqWsSession(new CqWsSessionOptions() {
                BaseUri = new Uri(uri),
            });

            var mp = _serviceProvider.GetRequiredService<IMsgPipeline>();
            _session.UseGroupMessage((context, next) =>
            {
                GroupMsg message = new();
                #region 赋值
                DateTime dtStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
                TimeSpan toNow = context.Time.Subtract(dtStart);
                long timeStamp = toNow.Ticks;
                long time = long.Parse(timeStamp.ToString().Substring(0, timeStamp.ToString().Length - 4));
                try {
                    message.BotType = BotType.QQbot;
                    message.Time = time;
                    message.SelfId = context.SelfId;
                    message.PostType = PostType.Message;
                    message.MsgType = MsgType.Group;
                    message.MsgSubType = MsgSubType.Group;
                    message.MsgID = context.MessageId;
                    message.UserID = context.UserId;
                    message.Msg = context.Message.Text;
                    message.RawMsg = context.RawMessage;
                    message.Font = context.Font;
                    message.NickName = context.Sender.Nickname;
                    message.Sex = context.Sender.Gender.ToString();
                    message.Age = context.Sender.Age;
                    message.GroupID = context.GroupId;
                    message.Card = context.Sender.Card;
                    message.Area = context.Sender.Area;
                    message.Level = context.Sender.Level;
                    message.Role = context.Sender.Role.ToString();
                    message.Title = context.Sender.Title;
                    //message.AnonymousID = context.Anonymous.Id;
                    //message.AnonymousName = context.Anonymous.Name;
                    //message.Flag = context.Anonymous.Flag;
                }
                catch (Exception ex) {
                    _logger.Warn(ex, "数据包转换出错");
                }
                

                #endregion
                mp.RecvGroupMsg(message);
                next();
            });

            _session.UsePrivateMessage((context,next) =>
            {
                PrivateMsg message = new PrivateMsg();

                DateTime dtStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
                TimeSpan toNow = context.Time.Subtract(dtStart);
                long timeStamp = toNow.Ticks;
                long time = long.Parse(timeStamp.ToString().Substring(0, timeStamp.ToString().Length - 4)); 
                try {
                    message.BotType = BotType.QQbot;
                    message.Time = time;
                    message.SelfId = context.SelfId;
                    message.PostType = PostType.Message;
                    message.MsgType = MsgType.Private;
                    message.MsgSubType = MsgSubType.Friend;
                    message.MsgID = context.MessageId;
                    message.UserID = context.UserId;
                    message.Msg = context.Message.Text;
                    message.RawMsg = context.RawMessage;
                    message.Font = context.Font;
                    message.NickName = context.Sender.Nickname;
                    message.Sex = context.Sender.Gender.ToString();
                    message.Age = context.Sender.Age;

                    message.TempSource = (int)context.TempSource;
                }
                catch (Exception ex) {
                    _logger.Warn(ex, "数据包转换出错");
                }
                
                mp.RecvPrivateMsg(message);
                next();
            });

            _session.UseFriendMessageRecalled((context, next) =>
            {
                FriendRecallNotice notice = new FriendRecallNotice();

                DateTime dtStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
                TimeSpan toNow = context.Time.Subtract(dtStart);
                long timeStamp = toNow.Ticks;
                long time = long.Parse(timeStamp.ToString().Substring(0, timeStamp.ToString().Length - 4));
                try {
                    notice.BotType = BotType.QQbot;
                    notice.Time = time;
                    notice.SelfId = context.SelfId;
                    notice.PostType = PostType.Notice;
                    notice.NoticeType = NoticeType.FriendRecall;
                    notice.UserID = context.UserId;
                    notice.MessageID = context.MessageId;
                }
                catch (Exception ex) {
                    _logger.Warn(ex, "数据包转换出错");
                }

                mp.RecvFriendRecallNotice(notice);
                next();
            });

            _session.UseGroupMessageRecalled((context, next) =>
            {
                GroupRecallNotice notice = new GroupRecallNotice();

                DateTime dtStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
                TimeSpan toNow = context.Time.Subtract(dtStart);
                long timeStamp = toNow.Ticks;
                long time = long.Parse(timeStamp.ToString().Substring(0, timeStamp.ToString().Length - 4));
                try {
                    notice.BotType = BotType.QQbot;
                    notice.Time = time;
                    notice.SelfId = context.SelfId;
                    notice.PostType = PostType.Notice;
                    notice.NoticeType = NoticeType.GroupRecall;
                    notice.GroupID = context.GroupId;
                    notice.UserID = context.UserId;
                    notice.OperatorID = context.OperatorId;
                    notice.MessageID = context.MessageId;
                }
                catch (Exception ex) {
                    _logger.Warn(ex, "数据包转换出错");
                }

                mp.RecvGroupRecallNotice(notice);
                next();
            });

            _session.UseGroupMemberIncreased((context, next) =>
            {
                GroupIncreaseNotice notice = new GroupIncreaseNotice();

                DateTime dtStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
                TimeSpan toNow = context.Time.Subtract(dtStart);
                long timeStamp = toNow.Ticks;
                long time = long.Parse(timeStamp.ToString().Substring(0, timeStamp.ToString().Length - 4));
                try {
                    notice.BotType = BotType.QQbot;
                    notice.Time = time;
                    notice.SelfId = context.SelfId;
                    notice.PostType = PostType.Notice;
                    notice.NoticeType = NoticeType.GroupIncrease;
                    notice.UserID = context.UserId;
                    notice.GroupID=context.GroupId;
                    notice.OperatorID = context.OperatorId;
                    notice.SubType = context.ChangeType.ToString();
                }
                catch (Exception ex) {
                    _logger.Warn(ex, "数据包转换出错");
                }

                mp.RecvGroupIncreaseNotice(notice);
                next();
            });

            _session.UseGroupMemberDecreased((context, next) =>
            {
                GroupDecreaseNotice notice = new GroupDecreaseNotice();

                DateTime dtStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
                TimeSpan toNow = context.Time.Subtract(dtStart);
                long timeStamp = toNow.Ticks;
                long time = long.Parse(timeStamp.ToString().Substring(0, timeStamp.ToString().Length - 4));
                try {
                    notice.BotType = BotType.QQbot;
                    notice.Time = time;
                    notice.SelfId = context.SelfId;
                    notice.PostType = PostType.Notice;
                    notice.NoticeType = NoticeType.GroupDecrease;
                    notice.UserID = context.UserId;
                    notice.GroupID = context.GroupId;
                    notice.OperatorID = context.OperatorId;
                    notice.SubType = context.ChangeType.ToString();
                }
                catch (Exception ex) {
                    _logger.Warn(ex, "数据包转换出错");
                }

                mp.RecvGroupDecreaseNotice(notice);
                next();
            });

            _session.UseGroupAdministratorChanged((context, next) =>
            {
                GroupAdminNotice notice = new GroupAdminNotice();

                DateTime dtStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
                TimeSpan toNow = context.Time.Subtract(dtStart);
                long timeStamp = toNow.Ticks;
                long time = long.Parse(timeStamp.ToString().Substring(0, timeStamp.ToString().Length - 4));
                try {
                    notice.BotType = BotType.QQbot;
                    notice.Time = time;
                    notice.SelfId = context.SelfId;
                    notice.PostType = PostType.Notice;
                    notice.NoticeType = NoticeType.GroupAdmin;
                    notice.UserID = context.UserId;
                    notice.GroupID= context.GroupId;
                    notice.SubType = context.ChangeType.ToString();
                }
                catch (Exception ex) {
                    _logger.Warn(ex, "数据包转换出错");
                }

                mp.RecvGroupAdminNotice(notice);
                next();
            });

            _session.UseGroupMemberBanChanged((context, next) =>
            {
                GroupBanNotice notice = new GroupBanNotice();

                DateTime dtStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
                TimeSpan toNow = context.Time.Subtract(dtStart);
                long timeStamp = toNow.Ticks;
                long time = long.Parse(timeStamp.ToString().Substring(0, timeStamp.ToString().Length - 4));
                try {
                    notice.BotType = BotType.QQbot;
                    notice.Time = time;
                    notice.SelfId = context.SelfId;
                    notice.PostType = PostType.Notice;
                    notice.NoticeType = NoticeType.GroupBan;
                    notice.UserID = context.UserId;
                    notice.GroupID = context.GroupId;
                    notice.OperatorID = context.OperatorId;
                    notice.SubType = context.ChangeType.ToString();
                    notice.Duration = context.Duration.Milliseconds;
                }
                catch (Exception ex) {
                    _logger.Warn(ex, "数据包转换出错");
                }

                mp.RecvGroupBanNotice(notice);
                next();
            });

            _session.UseFriendAdded((context, next) =>
            {
                FriendAddNotice notice = new FriendAddNotice();

                DateTime dtStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
                TimeSpan toNow = context.Time.Subtract(dtStart);
                long timeStamp = toNow.Ticks;
                long time = long.Parse(timeStamp.ToString().Substring(0, timeStamp.ToString().Length - 4));
                try {
                    notice.BotType = BotType.QQbot;
                    notice.Time = time;
                    notice.SelfId = context.SelfId;
                    notice.PostType = PostType.Notice;
                    notice.NoticeType = NoticeType.FriendAdd;
                    notice.UserID = context.UserId;
                }
                catch (Exception ex) {
                    _logger.Warn(ex, "数据包转换出错");
                }

                mp.RecvFriendAddNotice(notice);
                next();
            });

            _session.UsePoked((context, next) =>
            {
                PokeNotice notice = new PokeNotice();

                DateTime dtStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
                TimeSpan toNow = context.Time.Subtract(dtStart);
                long timeStamp = toNow.Ticks;
                long time = long.Parse(timeStamp.ToString().Substring(0, timeStamp.ToString().Length - 4));
                try {
                    notice.BotType = BotType.QQbot;
                    notice.Time = time;
                    notice.SelfId = context.SelfId;
                    notice.PostType = PostType.Notice;
                    notice.NoticeType = NoticeType.Poke;
                    notice.UserID = context.UserId;
                    notice.GroupID = (long)context.GroupId;
                    notice.TargetID = context.TargetId;
                }
                catch (Exception ex) {
                    _logger.Warn(ex, "数据包转换出错");
                }

                mp.RecvGroupPokeNotice(notice);
                next();
            });

            _session.UseGroupMemberHonorChanged((context, next) =>
            {
                HonorNotice notice = new HonorNotice();

                DateTime dtStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
                TimeSpan toNow = context.Time.Subtract(dtStart);
                long timeStamp = toNow.Ticks;
                long time = long.Parse(timeStamp.ToString().Substring(0, timeStamp.ToString().Length - 4));
                try {
                    notice.BotType = BotType.QQbot;
                    notice.Time = time;
                    notice.SelfId = context.SelfId;
                    notice.PostType = PostType.Notice;
                    notice.NoticeType = NoticeType.Honor;
                    notice.UserID = context.UserId;
                    notice.GroupID = context.GroupId;
                    notice.HonorType = context.HonorType.ToString();
                }
                catch (Exception ex) {
                    _logger.Warn(ex, "数据包转换出错");
                }

                mp.RecvHonorNotice(notice);
                next();
            });

            _session.UseGroupMemberTitleChangeNoticed((context, next) =>
            {
                GroupTitleNotice notice = new GroupTitleNotice();

                DateTime dtStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
                TimeSpan toNow = context.Time.Subtract(dtStart);
                long timeStamp = toNow.Ticks;
                long time = long.Parse(timeStamp.ToString().Substring(0, timeStamp.ToString().Length - 4));
                try {
                    notice.BotType = BotType.QQbot;
                    notice.Time = time;
                    notice.SelfId = context.SelfId;
                    notice.PostType = PostType.Notice;
                    notice.NoticeType = NoticeType.Title;
                    notice.UserID = context.UserId;
                    notice.GroupID = context.GroupId;
                    notice.Title = context.NewTitle;
                }
                catch (Exception ex) {
                    _logger.Warn(ex, "数据包转换出错");
                }

                mp.RecvTitleNotice(notice);
                next();
            });

            _session.UseFriendRequest((context, next) =>
            {
                FriendRequest request = new FriendRequest();

                DateTime dtStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
                TimeSpan toNow = context.Time.Subtract(dtStart);
                long timeStamp = toNow.Ticks;
                long time = long.Parse(timeStamp.ToString().Substring(0, timeStamp.ToString().Length - 4));
                try {
                    request.BotType = BotType.QQbot;
                    request.Time = time;
                    request.SelfId = context.SelfId;
                    request.PostType = PostType.Request;
                    request.RequestType = RequestType.Friend;
                    request.UserID = context.UserId;
                    request.Commnet = context.Comment;
                    request.Flag = context.Flag;
                }
                catch (Exception ex) {
                    _logger.Warn(ex, "数据包转换出错");
                }

                mp.RecvFriendRequest(request);
                next();
            });

            _session.UseGroupRequest((context, next) =>
            {
                GroupRequest request = new GroupRequest();

                DateTime dtStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
                TimeSpan toNow = context.Time.Subtract(dtStart);
                long timeStamp = toNow.Ticks;
                long time = long.Parse(timeStamp.ToString().Substring(0, timeStamp.ToString().Length - 4));
                try {
                    request.BotType = BotType.QQbot;
                    request.Time = time;
                    request.SelfId = context.SelfId;
                    request.PostType = PostType.Request;
                    request.RequestType = RequestType.Group;
                    request.UserID = context.UserId;
                    request.Comment = context.Comment;
                    request.GroupID = context.GroupId;
                    request.Flag = context.Flag;
                    request.SubType=context.GroupRequestType.ToString();
                }
                catch (Exception ex) {
                    _logger.Warn(ex, "数据包转换出错");
                }

                mp.RecvGroupRequest(request);
                next();
            });

            await _session.RunAsync();
            return;
        }
    }
}
