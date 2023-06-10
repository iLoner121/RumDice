using CSScripting;
using EleCho.GoCqHttpSdk;
using Microsoft.Extensions.DependencyInjection;
using RumDice.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RumDice.Core {
    public class ClientConnector : IClientConnector {
        public CqWsSession Session { get; set; } 
        readonly IServiceProvider _serviceProvider;
        readonly IRumLogger _logger;

        public ClientConnector(IServiceProvider serviceProvider,IRumLogger rumLogger) { 
            _serviceProvider = serviceProvider;
            _logger = rumLogger;
        }

        public async ValueTask SendPrivateMsg(Send send) {
            Session.SendPrivateMessage(send.UserID, new EleCho.GoCqHttpSdk.Message.CqMessage(send.Msg));
            return;
        }
        public async ValueTask SendGroupMsg(Send send) {
            Session.SendGroupMessage(send.GroupID, new EleCho.GoCqHttpSdk.Message.CqMessage(send.Msg));
            return;
        }

        public async ValueTask RunServer(string uri) {
            Session = new CqWsSession(new CqWsSessionOptions() {
                BaseUri = new Uri(uri),
            });

            var mp = _serviceProvider.GetRequiredService<IMsgPipeline>();
            Session.UseGroupMessage((context, next) =>
            {
                GroupMsg message = new();
                #region 赋值
                DateTime dtStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
                TimeSpan toNow = context.Time.Subtract(dtStart);
                long timeStamp = toNow.Ticks;
                long time = long.Parse(timeStamp.ToString().Substring(0, timeStamp.ToString().Length - 4));
                try {
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

            Session.UsePrivateMessage((context,next) =>
            {
                PrivateMsg message = new PrivateMsg();

                DateTime dtStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
                TimeSpan toNow = context.Time.Subtract(dtStart);
                long timeStamp = toNow.Ticks;
                long time = long.Parse(timeStamp.ToString().Substring(0, timeStamp.ToString().Length - 4)); 
                try {
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

            Session.UseFriendMessageRecalled((context, next) =>
            {
                FriendRecallNotice notice = new FriendRecallNotice();

                DateTime dtStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
                TimeSpan toNow = context.Time.Subtract(dtStart);
                long timeStamp = toNow.Ticks;
                long time = long.Parse(timeStamp.ToString().Substring(0, timeStamp.ToString().Length - 4));
                try {
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

            Session.UseGroupMessageRecalled((context, next) =>
            {
                GroupRecallNotice notice = new GroupRecallNotice();

                DateTime dtStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
                TimeSpan toNow = context.Time.Subtract(dtStart);
                long timeStamp = toNow.Ticks;
                long time = long.Parse(timeStamp.ToString().Substring(0, timeStamp.ToString().Length - 4));
                try {
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

            Session.UseGroupMemberIncreased((context, next) =>
            {
                GroupIncreaseNotice notice = new GroupIncreaseNotice();

                DateTime dtStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
                TimeSpan toNow = context.Time.Subtract(dtStart);
                long timeStamp = toNow.Ticks;
                long time = long.Parse(timeStamp.ToString().Substring(0, timeStamp.ToString().Length - 4));
                try {
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

            Session.UseGroupMemberDecreased((context, next) =>
            {
                GroupDecreaseNotice notice = new GroupDecreaseNotice();

                DateTime dtStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
                TimeSpan toNow = context.Time.Subtract(dtStart);
                long timeStamp = toNow.Ticks;
                long time = long.Parse(timeStamp.ToString().Substring(0, timeStamp.ToString().Length - 4));
                try {
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

            Session.UseGroupAdministratorChanged((context, next) =>
            {
                GroupAdminNotice notice = new GroupAdminNotice();

                DateTime dtStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
                TimeSpan toNow = context.Time.Subtract(dtStart);
                long timeStamp = toNow.Ticks;
                long time = long.Parse(timeStamp.ToString().Substring(0, timeStamp.ToString().Length - 4));
                try {
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

            Session.UseGroupMemberBanChanged((context, next) =>
            {
                GroupBanNotice notice = new GroupBanNotice();

                DateTime dtStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
                TimeSpan toNow = context.Time.Subtract(dtStart);
                long timeStamp = toNow.Ticks;
                long time = long.Parse(timeStamp.ToString().Substring(0, timeStamp.ToString().Length - 4));
                try {
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

            Session.UseFriendAdded((context, next) =>
            {
                FriendAddNotice notice = new FriendAddNotice();

                DateTime dtStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
                TimeSpan toNow = context.Time.Subtract(dtStart);
                long timeStamp = toNow.Ticks;
                long time = long.Parse(timeStamp.ToString().Substring(0, timeStamp.ToString().Length - 4));
                try {
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

            Session.UsePoked((context, next) =>
            {
                PokeNotice notice = new PokeNotice();

                DateTime dtStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
                TimeSpan toNow = context.Time.Subtract(dtStart);
                long timeStamp = toNow.Ticks;
                long time = long.Parse(timeStamp.ToString().Substring(0, timeStamp.ToString().Length - 4));
                try {
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

            Session.UseGroupMemberHonorChanged((context, next) =>
            {
                HonorNotice notice = new HonorNotice();

                DateTime dtStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
                TimeSpan toNow = context.Time.Subtract(dtStart);
                long timeStamp = toNow.Ticks;
                long time = long.Parse(timeStamp.ToString().Substring(0, timeStamp.ToString().Length - 4));
                try {
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

            Session.UseGroupMemberTitleChangeNoticed((context, next) =>
            {
                GroupTitleNotice notice = new GroupTitleNotice();

                DateTime dtStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
                TimeSpan toNow = context.Time.Subtract(dtStart);
                long timeStamp = toNow.Ticks;
                long time = long.Parse(timeStamp.ToString().Substring(0, timeStamp.ToString().Length - 4));
                try {
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

            Session.UseFriendRequest((context, next) =>
            {
                FriendRequest request = new FriendRequest();

                DateTime dtStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
                TimeSpan toNow = context.Time.Subtract(dtStart);
                long timeStamp = toNow.Ticks;
                long time = long.Parse(timeStamp.ToString().Substring(0, timeStamp.ToString().Length - 4));
                try {
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

            Session.UseGroupRequest((context, next) =>
            {
                GroupRequest request = new GroupRequest();

                DateTime dtStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
                TimeSpan toNow = context.Time.Subtract(dtStart);
                long timeStamp = toNow.Ticks;
                long time = long.Parse(timeStamp.ToString().Substring(0, timeStamp.ToString().Length - 4));
                try {
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
            



            


            await Session.RunAsync();
            return;
        }
    }
}
