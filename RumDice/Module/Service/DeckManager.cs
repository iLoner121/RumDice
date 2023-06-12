using CSScriptLib;
using RumDice.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace RumDice.Module {
    public class DeckManager :IDeckManager {
        // 该实例仅用于初始化，平时会被设为null
        private Deck? deck;

        // 有权重的抽牌
        string DrawWithWeight(List<string> list) {
            List<int> weight = new();
            List<string> cards = new();
            // 识别文本中的权重项（无权重默认为1）
            string pattern = @"\((.*?)%\)";
            foreach (var s in list) {
                var match = Regex.Match(s, pattern).Groups[1].Value;
                int w;
                if (int.TryParse(match, out w)) {
                    cards.Add(s.Replace("(" + match + "%)", ""));
                    weight.Add(w);
                } else {
                    w = 1;
                    cards.Add(s);
                    weight.Add(w);
                }
            }
            // 计算总权重值
            int weightAll = 0;
            foreach (int w in weight) {
                if (w < 0) {
                    RumLogger.Instance.Warn("DrawCard", "权重数值出错");
                    return "";
                }
                weightAll += w;
            }
            // 求得加权随机数
            int add = 0;
            int n = new Random().Next(1, weightAll + 1);
            for (int i = 0; i < weight.Count; i++) {
                add += weight[i];
                if (n <= add) {
                    return cards[i];
                }
            }
            return null;
        }

        // 回复接口牌堆列表
        public string DeckList(Post post) {
            if (DataCenter.Instance.GetObj("\\Deck\\AllDeck.json") is not Deck d)
                return "牌堆出错了哦！";
            string decklist = "";
            int count= 0;
            foreach(var name in d.table.Keys) {
                if (name.StartsWith("_"))
                    continue;
                decklist += name;
                decklist += "\n";
                count++;
            }
            var pl = new Dictionary<string, string>();
            pl.Add("牌堆列表", decklist);
            pl.Add("牌堆数量", ""+count);
            return new MsgTool().GenerateMsg("RumDice.Module.IDeckManager.DeckList", pl);
        }

        // 回复接口抽卡
        public string DrawCard(Post post) {
            var mt = new MsgTool();
            var msg = mt.GetTextMsg(post);
            var deckname = mt.GetMsgWithoutPrefix(msg, "draw");
            bool isRepeat = false;
            if(deckname.StartsWith('%')) {
                isRepeat = true;
                deckname = deckname.Substring(1);
            }
            // 查找是否有该牌堆
            if (DataCenter.Instance.GetObj("\\Deck\\AllDeck.json") is not Deck d)
                return "牌堆出错了哦！";
            if(!d.table.ContainsKey(deckname)) {
                return "没有这个名字的牌堆哦";
            }
            // 将回复字符串的结果替换为内置服务
            string res = "{" + (isRepeat ? "%draw:" : "draw:") + deckname + "}";
            return mt.GenerateMsg("RumDice.Module.IDeckManager.DrawCard", new List<string> { res });
        }

        // 内置服务抽卡
        public string DrawCard(string command) {
            if (DataCenter.Instance.GetObj("\\Deck\\AllDeck.json") is not Deck d)
                return "";
            var deckname = new MsgTool().GetMsgWithoutPrefix(command, "draw:");

            // 寻找匹配牌组
            foreach (var t in d.table) {
                string key = t.Key;
                if (key.StartsWith("_")&&!deckname.StartsWith('_'))
                    key = key.Substring(1);

                if (key.Equals(deckname)) {
                    return DrawWithWeight(t.Value);
                }
            }

            return "";
        }


        // 牌组功能初始化
        public void Initialize(Post post) {
            // 牌堆地址默认值
            string location = "\\Deck";
            string allDeck = "\\AllDeck.json";

            var dataCenter = DataCenter.Instance;
            deck=new Deck();
            // 尝试获取牌堆总表格
            if(dataCenter.TryGetObj(location + allDeck, out object obj))
                if (obj is Deck) 
                    deck = (Deck)obj;

            // 添加初始牌堆
            if (deck.table.Count == 0) {
                deck.table.Add("朗姆骰", new List<string> {
                    "赛高！",
                    "崭新营业！",
                    "肝到通宵！",
                    "肝到通宵！",
                    "肝到通宵！",
                    "肝到通宵！",
                    "肝到通宵！！！",
                });
            }
            // 设置remark
            deck.remark = (deck.remark == null||deck.remark==string.Empty) ? "汇总牌堆" : deck.remark;
            // 用本地其他牌堆的内容填补汇总牌堆
            var otherDeck = dataCenter.GetByType("Deck");
            foreach(var item in otherDeck) {
                if (item is not Deck)
                    continue;
                foreach (var sd in ((Deck)item).table) {
                    // 如果有相同的牌堆名称，则将内容不保留重复项地合并
                    if (deck.table.ContainsKey(sd.Key)) {
                        deck.table[sd.Key] = deck.table[sd.Key].Union(sd.Value).ToList<string>();
                    } else {
                        deck.table.Add(sd.Key, sd.Value);
                    }
                }
            }
            // 尝试获取没有标注为牌堆的数据类型
            dataCenter.ScanFile(location, typeof(Dictionary<string, List<string>>), MergeDeck);

            dataCenter.SaveFile<Deck>(deck,location+allDeck,3);
            deck = null;
        }

        // 在系统启动的时候调用
        public void MergeDeck(object obj) {
            if (obj is not Dictionary<string,List<string>> d)
                return;
            foreach (var item in d) {
                // 如果有相同的牌堆名称，则将内容不保留重复项地合并
                if (deck.table.ContainsKey(item.Key)) {
                    deck.table[item.Key] = deck.table[item.Key].Union(item.Value).ToList<string>();
                } else {
                    deck.table.Add(item.Key, item.Value);
                }
            }
        }
    }
}
