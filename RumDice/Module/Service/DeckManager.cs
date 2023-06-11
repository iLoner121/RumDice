using CSScriptLib;
using RumDice.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;

namespace RumDice.Module {
    public class DeckManager : IDeckManager {
        // 该实例仅用于初始化，平时会被设为null
        private Deck? deck;
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

        public string DrawCard(Post post) {
            var mt = new MsgTool();
            var msg = mt.GetTextMsg(post);
            var deckname = mt.GetMsgWithoutPrefix(msg, "draw");
            if (DataCenter.Instance.GetObj("\\Deck\\AllDeck.json") is not Deck d)
                return "牌堆出错了哦！";
            if(!d.table.ContainsKey(deckname)) {
                return "没有这个名字的牌堆哦";
            }
            
            var n = new Random().Next(d.table[deckname].Count);
            var res = d.table[deckname][n];
            return mt.GenerateMsg("RumDice.Module.IDeckManager.DrawCard", new List<string> { res });
        }

        public string DrawCard(string command) {
            if (DataCenter.Instance.GetObj("\\Deck\\AllDeck.json") is not Deck d)
                return "";
            var deckname = new MsgTool().GetMsgWithoutPrefix(command, "draw:");

            foreach (var t in d.table) {
                string key = t.Key;
                if (key.StartsWith("_")&&!deckname.StartsWith('_'))
                    key = key.Substring(1);

                if (key.Equals(deckname)) {
                    var n = new Random().Next(t.Value.Count);
                    return t.Value[n];
                }
            }

            return "";
        }

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

        public void MergeDeck(object obj) {
            if (obj is not Deck)
                return;
            var d = (Deck)obj;
            foreach (var item in d.table) {
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
