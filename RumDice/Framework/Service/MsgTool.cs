using RumDice.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace RumDice.Framework {
    public class MsgTool : IMsgTool {
        public string GenerateMsg(string fullname, List<string> paramList) {
            var returnWord = ((ReturnWordTable)DataCenter.Instance.GetObj(CoreData.Instance.Setting.FileConfig.ReturnWordTable)).table[fullname];
            for(int i = 0; i < paramList.Count; i++) {
                string temp = "{" + i + "}";
                if (returnWord.Contains(temp)) {
                    RumLogger.Instance.Debug("MessageTool", $"在回复语句中识别到参数{temp}，将替换为\"{paramList[i]}\"");
                    returnWord = returnWord.Replace(temp, paramList[i]);
                }
            }
            RumLogger.Instance.Debug("MessageTool", "回复语句已生成");
            return returnWord;
        }

        public string GenerateMsg(string fullname, Dictionary<string, string> paramList) {
            var returnWord = ((ReturnWordTable)DataCenter.Instance.GetObj(CoreData.Instance.Setting.FileConfig.ReturnWordTable)).table[fullname];
            foreach(var r in paramList) {
                string temp = "{" + r.Key + "}";
                if (returnWord.Contains(temp)) {
                    RumLogger.Instance.Debug("MessageTool", $"在回复语句中识别到参数{temp}，将替换为\"{r.Value}\"");
                    returnWord = returnWord.Replace(temp, r.Value);
                }
            }
            RumLogger.Instance.Debug("MessageTool", "回复语句已生成");
            return returnWord;
        }

        public MsgType GetMsgType(Post post) {
            BaseMsg baseMsg= (BaseMsg)post;
            return baseMsg.MsgType;
        }

        public string GetMsgWithoutPrefix(Post post,string prefix) {
            BaseMsg baseMsg = (BaseMsg)post;
            string msg=baseMsg.Msg;
            int index=msg.IndexOf(prefix,StringComparison.OrdinalIgnoreCase);
            int finalIndex=-1;
            for(int i = index + prefix.Length; i < msg.Length; i++) {
                if (msg[i] == ' ') continue;
                finalIndex = i;
                break;
            }
            if (finalIndex == -1) return "";
            return msg.Substring(finalIndex);
        }

        public string GetTextMsg(Post post) {
            BaseMsg baseMsg = (BaseMsg)post;
            string msg = baseMsg.Msg;
            return msg;
        }

        public Send MakeSend(string msg, Post post) {
            var send = new Send();
            send.Msg = msg;
            send.MsgType=GetMsgType(post);
            switch (send.MsgType) {
                case MsgType.Private:
                    var pm=(PrivateMsg)post;
                    send.UserID = pm.UserID;
                    break;
                case MsgType.Group:
                    var gm=(GroupMsg)post;
                    send.GroupID = gm.GroupID;
                    break;
                default:
                    break;
            }
            return send;
        }

        public List<Send> MakeSend(List<string> msgs, Post post) {
            var res = new List<Send>();
            foreach (string msg in msgs) {
                res.Add(MakeSend(msg, post));
            }
            return res;
        }
    }
}
