using RumDice.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace RumDice.Framework {
    public class MessageTool : IMessageTool {
        public string GenerateMessage(string fullname, List<string> paramList) {
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

        public string GenerateMessage(string fullname, Dictionary<string, string> paramList) {
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

        public MessageType GetMsgType(Post post) {
            BaseMessage baseMsg= (BaseMessage)post;
            return baseMsg.MsgType;
        }

        public string GetMsgWithoutPrefix(Post post) {
            throw new NotImplementedException();
        }

        public string GetTextMsg(Post post) {
            throw new NotImplementedException();
        }

        public Send MakeSend(string msg, Post post) {
            throw new NotImplementedException();
        }

        public List<Send> MakeSends(List<string> msgs, Post post) {
            throw new NotImplementedException();
        }
    }
}
