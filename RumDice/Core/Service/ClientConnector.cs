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
            Session.UseGroupMessage((context, next) =>
            {
                GroupMessage message = new();
                #region 赋值
                DateTime dtStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
                TimeSpan toNow = context.Time.Subtract(dtStart);
                long timeStamp = toNow.Ticks;
                message.Time = long.Parse(timeStamp.ToString().Substring(0, timeStamp.ToString().Length - 4));
                message.SelfId = context.SelfId;
                message.PostType = PostType.Message;
                message.MsgType = MessageType.Group;
                message.MsgSubType = MessageSubType.Group;
                message.MsgID = context.MessageId;
                message.UserID = context.UserId;
                message.Msg = context.Message.Text;
                message.RawMsg = context.RawMessage;
                message.Font = context.Font;
                message.NickName = context.Sender.Nickname;
                message.Sex=context.Sender.Gender.ToString();
                message.Age = context.Sender.Age;
                message.GroupID = context.GroupId;
                message.Card = context.Sender.Card;
                message.Area=context.Sender.Area;
                message.Level= context.Sender.Level;
                message.Role=context.Sender.Role.ToString();
                message.Title=context.Sender.Title;
                message.AnonymousID = context.Anonymous.Id;
                message.AnonymousName= context.Anonymous.Name;
                message.Flag=context.Anonymous.Flag;

                #endregion
                _serviceProvider.GetRequiredService<IMessagePipeline>().RecvGroupMsg(message);
                next();
            });

            Session.UsePrivateMessage((context,next) =>
            {
                PrivateMessage message = new PrivateMessage();

                DateTime dtStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
                TimeSpan toNow = context.Time.Subtract(dtStart);
                long timeStamp = toNow.Ticks;
                message.Time = long.Parse(timeStamp.ToString().Substring(0, timeStamp.ToString().Length - 4));
                message.SelfId = context.SelfId;
                message.PostType = PostType.Message;
                message.MsgType = MessageType.Group;
                message.MsgSubType = MessageSubType.Group;
                message.MsgID = context.MessageId;
                message.UserID = context.UserId;
                message.Msg = context.Message.Text;
                message.RawMsg = context.RawMessage;
                message.Font = context.Font;
                message.NickName = context.Sender.Nickname;
                message.Sex = context.Sender.Gender.ToString();
                message.Age = context.Sender.Age;

                message.TempSource = (int)context.TempSource;

                _serviceProvider.GetRequiredService<IMessagePipeline>().RecvGroupMsg(message);
                //_eventManager.HandlePrivateMessage(message);
                next();
            });
            await Session.RunAsync();
            return;
        }
    }
}
