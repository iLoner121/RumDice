using Kook;
using Kook.WebSocket;
using Microsoft.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;
using RumDice.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace RumDice.Core {
    public class KookClientConnector : IBaseClient {
        KookSocketClient _client { get; set; }
        readonly IServiceProvider _serviceProvider;
        readonly IRumLogger _logger;

        public KookClientConnector(IServiceProvider serviceProvider, IRumLogger logger) {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }


        Task Log(LogMessage msg) {
            RumLogger.Instance.Info(msg.Source,msg.Message);
            return Task.CompletedTask;
        }

        public void RunServer(string token) {
            RunServerAsync(token).AsTask().Wait();
        }
        public async ValueTask RunServerAsync(string token) {
            var mp = _serviceProvider.GetRequiredService<IMsgPipeline>();

            _client = new KookSocketClient(new KookSocketConfig {
                AlwaysDownloadUsers = false,
                AlwaysDownloadVoiceStates = false,
                AlwaysDownloadBoostSubscriptions = false,
                MessageCacheSize = 100,
                LogLevel = LogSeverity.Debug
            });

            #region BaseKookClient

            _client.Log += Log;
            _client.LoggedIn += () => Task.CompletedTask;
            _client.LoggedOut += () => Task.CompletedTask;

            #endregion

            #region KookSocketClient

            _client.Connected += () => Task.CompletedTask;
            _client.Disconnected += exception => Task.CompletedTask;
            _client.Ready += () => Task.CompletedTask;
            _client.LatencyUpdated += (before, after) => Task.CompletedTask;

            #endregion

            #region BaseSocketClient 

            _client.ChannelCreated += channel => Task.CompletedTask;
            _client.ChannelDestroyed += channel => Task.CompletedTask;
            _client.ChannelUpdated += (before, after) => Task.CompletedTask;

            _client.ReactionAdded += (message, channel, user, reaction) => Task.CompletedTask;
            _client.ReactionRemoved += (message, channel, user, reaction) => Task.CompletedTask;
            _client.DirectReactionAdded += (message, channel, user, reaction) => Task.CompletedTask;
            _client.DirectReactionRemoved += (message, channel, user, reaction) => Task.CompletedTask;

            // 接收信息
            _client.MessageReceived += (message, author, channel) =>
            {
                if((bool)author.IsBot)
                    return Task.CompletedTask;

                GroupMsg msg = new GroupMsg();

                DateTime dtStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
                TimeSpan toNow = message.Timestamp.Subtract(dtStart);
                long timeStamp = toNow.Ticks;
                long time = long.Parse(timeStamp.ToString().Substring(0, timeStamp.ToString().Length - 4));
                try {
                    msg.BotType = BotType.KOOKbot;
                    msg.Time = time;
                    //msg.Selfid
                    msg.PostType = PostType.Message;
                    msg.MsgType = MsgType.Group;
                    msg.MsgSubType = MsgSubType.Group;
                    msg.UserID = (long)message.Author.Id;
                    msg.Msg = message.CleanContent;
                    msg.RawMsg = message.RawContent;
                    //msg.Font
                    msg.NickName = author.Username;
                    //msg.Sex
                    //msg.Age
                    msg.GroupID = (long)channel.Id;
                    msg.Card = author.Nickname!=null?author.Nickname:author.Username;
                }
                catch (Exception ex){
                    _logger.Warn(ex, "数据包转换出错");
                }

                mp.RecvGroupMsg(msg);

                return Task.CompletedTask;
            };
            _client.MessageDeleted += (message, channel) => Task.CompletedTask;
            _client.MessageUpdated += (before, after, channel) => Task.CompletedTask;
            _client.MessagePinned += (before, after, channel, @operator) => Task.CompletedTask;
            _client.MessageUnpinned += (before, after, channel, @operator) => Task.CompletedTask;

            _client.DirectMessageReceived += (message, author, channel) => {
                if ((bool)author.IsBot)
                    return Task.CompletedTask;

                PrivateMsg msg = new PrivateMsg();

                DateTime dtStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
                TimeSpan toNow = message.Timestamp.Subtract(dtStart);
                long timeStamp = toNow.Ticks;
                long time = long.Parse(timeStamp.ToString().Substring(0, timeStamp.ToString().Length - 4));
                try {
                    msg.BotType = BotType.KOOKbot;
                    msg.Time = time;
                    //msg.Selfid
                    msg.PostType = PostType.Message;
                    msg.MsgType = MsgType.Private;
                    msg.MsgSubType = MsgSubType.Friend;
                    msg.UserID = (long)message.Author.Id;
                    msg.Msg = message.CleanContent;
                    msg.RawMsg = message.RawContent;
                    //msg.Font
                    msg.NickName = author.Username;
                    //msg.Sex
                    //msg.Age
                }
                catch (Exception ex) {
                    _logger.Warn(ex, "数据包转换出错");
                }

                mp.RecvPrivateMsg(msg);

                return Task.CompletedTask;
            };
            _client.DirectMessageDeleted += (message, author, channel) => Task.CompletedTask;
            _client.DirectMessageUpdated += (before, after, author, channel) => Task.CompletedTask;

            _client.UserJoined += (user, time) => Task.CompletedTask;
            _client.UserLeft += (guild, user, time) => Task.CompletedTask;
            _client.UserBanned += (users, @operator, guild, reason) => Task.CompletedTask;
            _client.UserUnbanned += (users, @operator, guild) => Task.CompletedTask;
            _client.UserUpdated += (before, after) => Task.CompletedTask;
            _client.CurrentUserUpdated += (before, after) => Task.CompletedTask;
            _client.GuildMemberUpdated += (before, after) => Task.CompletedTask;
            _client.GuildMemberOnline += (users, time) => Task.CompletedTask;
            _client.GuildMemberOffline += (users, time) => Task.CompletedTask;

            _client.UserConnected += (user, channel, time) => Task.CompletedTask;
            _client.UserDisconnected += (user, channel, time) => Task.CompletedTask;

            _client.RoleCreated += role => Task.CompletedTask;
            _client.RoleDeleted += role => Task.CompletedTask;
            _client.RoleUpdated += (before, after) => Task.CompletedTask;

            _client.EmoteCreated += (emote, guild) => Task.CompletedTask;
            _client.EmoteDeleted += (emote, guild) => Task.CompletedTask;
            _client.EmoteUpdated += (before, after, guild) => Task.CompletedTask;

            _client.JoinedGuild += guild => {
                _client.StopAsync();
                _client.StartAsync();
                return Task.CompletedTask;
            };
            _client.LeftGuild += guild => Task.CompletedTask;
            _client.GuildUpdated += (before, after) => Task.CompletedTask;
            _client.GuildAvailable += guild => Task.CompletedTask;
            _client.GuildUnavailable += guild => Task.CompletedTask;

            _client.MessageButtonClicked += (value, user, message, channel) => Task.CompletedTask;
            _client.DirectMessageButtonClicked += (value, user, message, channel) => Task.CompletedTask;

            #endregion

            await _client.LoginAsync(TokenType.Bot,token);
            await _client.StartAsync();
        }

        public void SendGroupMsg(Send send) {
            string referenceId = null;
            var channel = _client.GetChannel((ulong)send.GroupID);
            if (channel is SocketTextChannel textChannel) {
                if(send is not KookSend ks) {
                    var result = textChannel.SendTextAsync(send.Msg);
                } else {
                    if (ks.KookMsgType==KookMsgType.Code) {
                        send.Msg = $"`{send.Msg}`";
                    }
                    var result = textChannel.SendTextAsync(send.Msg);
                }
                return;
            }
            return;
        }

        public void SendPrivateMsg(Send send) {
            var user = _client.GetUser((ulong)send.UserID);
            if(user is SocketUser u) {
                if(send is not KookSend ks) {
                    var result = u.SendTextAsync(send.Msg);
                } else {
                    if(ks.KookMsgType == KookMsgType.Code) {
                        send.Msg = $"`{send.Msg}`";
                    }
                    var result = u.SendTextAsync(send.Msg);
                }
                return;
            }
            return;
        }
    }
}
