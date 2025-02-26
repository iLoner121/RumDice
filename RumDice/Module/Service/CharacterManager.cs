using CSScriptLib;
using RumDice.Core;
using RumDice.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace RumDice.Module{
    public class CharacterManager: ICharacterManager{

        private string moduleName = "CharacterManager";
        private Character? character;
        private CharacterList? characterList;
        private string local = "RumDice.Module.ICharacterManager.{0}";

        public string ShowCharacterList(Post post){
            if (DataCenter.Instance.GetObj("\\Character\\CharacterList.json") is not CharacterList c)
                return "角色列表出错了！";
            string res = "";
            int cnt = 0;
            foreach (string name in c.table.Keys){
                if (cnt != 0)
                    res += "\n";
                cnt += 1;
                res += cnt.ToString() + " " + name;
            }
            return new MsgTool().GenerateMsg(string.Format(local, "ShowCharacterList"), new List<string>{res});
        }

        public string CharacterChange(Post post){
            IMsgTool mt = new MsgTool();
            IDataCenter dataCenter = DataCenter.Instance;
            IRumLogger rumLogger = RumLogger.Instance;
            var coreData = CoreData.Instance;
            ReturnWordTable newRes = new ReturnWordTable();
            string location = "\\Character";
            string list = "\\CharacterList.json";

            string name = "";
            foreach (char i in mt.GetTextMsg(post)){
                if (i == '.')
                    break;
                name += i;
            }
            if (dataCenter.TryGetObj(location+list, out object obj)){
                if (obj is CharacterList)
                    characterList = (CharacterList)obj;
                else{
                    rumLogger.Debug(this.moduleName, "角色表异常");
                    return "切换失败";
                }
            }
            if (characterList.table.ContainsKey(name)){
                character = characterList.table[name];
            }
            else{
                return "不存在此角色";
            }
            foreach (var i in character.table){
                Dictionary<string, string> dic = new Dictionary<string, string>();
                dic.Add("default", i.Value);
                newRes.table.Add(i.Key, dic);
            }
            coreData.ReLoadRes(newRes, moduleName);
            character = null;
            characterList = null;
            return mt.GenerateMsg(string.Format(local, "CharacterChange"), new List<string>());
        }

        public void Initialize(Post post){
            string location = "\\Character";
            string list = "\\CharacterList.json";

            var dataCenter = DataCenter.Instance;
            characterList = new CharacterList();
            if (dataCenter.TryGetObj(location + list, out object obj))
                if (obj is CharacterList) 
                    characterList = (CharacterList)obj;
            // character = new Character();
            // character.remark = "德洛莉丝";
            // character.table.Add(string.Format(local, "ShowCharacterList"), "角色列表\n{0}");
            // characterList.table.Add("德洛莉丝", character);
            // characterList.remark = "角色列表";
            var otherCharacter = dataCenter.GetObjByType("Character");
            foreach (var i in otherCharacter){
                if (i is not Character){
                    continue;
                }
                character = (Character)i;
                if (characterList.table.ContainsKey(character.name)){
                    // 后覆盖
                    foreach (var j in character.table){
                        characterList.table[character.name].table[j.Key] = j.Value;
                    }
                }
                else{
                    characterList.table.Add(character.name, character);
                }
            }
            

            //todo: 自定义启动角色
            if (characterList.table.ContainsKey("德洛莉丝")){
                character = characterList.table["德洛莉丝"];
                ReturnWordTable iniRes = new ReturnWordTable();
                foreach (var i in character.table){
                    Dictionary<string, string> dic = new Dictionary<string, string>();
                    dic.Add("default", i.Value);
                    iniRes.table.Add(i.Key, dic);
                }
                var coreData = CoreData.Instance;
                coreData.ReLoadRes(iniRes, moduleName);
            }

            dataCenter.SaveFile<CharacterList>(characterList, location+list, ReadType:1);
            character = null;
            characterList = null;
        }
    }
}