using CSScriptLib;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RumDice.Framework.Datatype;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace RumDice.Framework {
    public class DataCenter : IDataCenter {
        readonly IServiceManager _serviceManager;
         string _root=null;

        public DataCenter(IServiceManager serviceManager) {
            _serviceManager = serviceManager;
        }
        public Dictionary<string, MyObjInfo> ObjTable { get; }=new();

        public Dictionary<string, MyFileInfo> FileTable { get; }=new();

        public List<object> GetAll() {
            return ObjTable.Select(x => x.Value.Obj).ToList();
        }

        public List<object> GetAllInnerFile() {
            return ObjTable.Where(x=>x.Value.IsPlugin==false).Select(x=>x.Value.Obj).ToList();
        }

        public List<object> GetAllPluginFile() {
            return ObjTable.Where(x=>x.Value.IsPlugin==true).Select(x=>x.Value.Obj).ToList();
        }

        public object GetByName(string name) {
            name = (name.EndsWith(".json") ? name : (name + ".json"));
            if (!FileTable.ContainsKey(name))
                return null;
            if (FileTable[name].ReadType == 3) {
                if (!ObjTable.ContainsKey(name)) {
                    int n;
                    Type t;
                    var obj = ReadFile(FileTable[name].Location, out n, out t);
                    if (obj == null) {
                        return null;
                    }
                    var objInfo = new MyObjInfo();
                    objInfo.Name = name;
                    objInfo.IsPlugin = false;
                    objInfo.Obj = obj;

                    ObjTable.Add(name, objInfo);
                }
                return ObjTable[name];
            }
            if (FileTable[name].ReadType == 2) {
                if (!ObjTable.ContainsKey(name)) {
                    int n;
                    Type t;
                    var obj = ReadFile(FileTable[name].Location,out n,out t);
                    if(obj == null) {
                        return null;
                    }
                    var objInfo =new MyObjInfo();
                    objInfo.Name = name;
                    objInfo.IsPlugin = false;
                    objInfo.Obj = obj;

                    ObjTable.Add(name, objInfo );
                }
                var tempObj = ObjTable[name];
                int tempUsed = ++ObjTable[name].UsedTime;
                int maxUsed = tempObj.MaxTime;
                if (tempUsed > maxUsed) {
                    ObjTable.Remove(name);
                }
                return tempObj.Obj;
            }
            if (FileTable[name].ReadType==3) {
                int n;
                Type t;
                var obj = ReadFile(FileTable[name].Location, out n, out t);
                if(obj == null) {
                    return null;
                }
                return obj;
            }
            return null;
        }


        public List<object> GetByType(string type) {
            throw new NotImplementedException();
        }

        private object ReadFile(string path,out int readType,out Type objType) {
            string JsonString = File.ReadAllText(path, Encoding.UTF8);
            readType = -1;
            objType = null;
            JObject jObj = JObject.Parse(JsonString);
            try {
                string typeName = jObj["fileType"].ToString();
                bool hasReadType = int.TryParse(jObj["readType"].ToString(), out readType);
                if(!hasReadType) 
                    readType = 1;

                Type type;
                var tempObj = _serviceManager.GetStruct(typeName,out type);
                tempObj = JsonConvert.DeserializeObject(JsonString);

                objType = type;
                return tempObj;
            }
            catch {
                Console.WriteLine("读取文件失败");
                return null;
            }
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public bool SaveFile(object obj, Type type,string name, int readType = -1) {
            name = (name.EndsWith(".json") ? name : ( name + ".json"));
            if (FileTable.ContainsKey(name)) {
                if (FileTable[name].ObjType != type) {
                    Console.WriteLine("存储失败");
                    return false;
                }
                if (readType == -1)
                    readType = FileTable[name].ReadType;
            } else {
                if (readType == -1)
                    readType = 3;

                var fileInfo = new MyFileInfo();
                fileInfo.Name = name;
                fileInfo.ReadType = readType;
                fileInfo.Location = _root + "\\" + name;
                fileInfo.ObjType = type;
                FileTable.Add(name, fileInfo);
            }
            if(ObjTable.ContainsKey(name)) {
                ObjTable.Remove(name);
            }
            try {
                string jsonString = JsonConvert.SerializeObject(obj);

                JObject jObj = JObject.Parse(jsonString);
                if (!jObj.ContainsKey("readType")) {
                    jObj.Add(new JProperty("readType",readType));
                }
                if (!jObj.ContainsKey("fileType")) {
                    jObj.Add(new JProperty("fileType", type.Name));
                }
                jsonString= JsonConvert.SerializeObject(jObj);

                Console.WriteLine(jsonString);
                FileStream fs = new FileStream(FileTable[name].Location, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);
                fs.Seek(0, SeekOrigin.Begin);
                fs.SetLength(0);
                StreamWriter sw = new StreamWriter(fs, Encoding.UTF8);
                sw.WriteLine(jsonString);
                sw.Close();
                return true;
            }
            catch {
                return false;
            }
        }

        public void Initialize(string root) {
            _root=root;
        }

        public async ValueTask ScanFile() {
            DirectoryInfo dir = new DirectoryInfo(_root);
            var files = dir.GetFiles("*.json", System.IO.SearchOption.AllDirectories);
            foreach ( var file in files ) {
                int readType = -1;
                Type type = null;
                var tempObj=ReadFile(file.FullName,out readType,out type);

                if(tempObj==null) {
                    continue;
                }

                var fileInfo = new MyFileInfo();
                fileInfo.Location=file.FullName;
                fileInfo.Name=file.Name;
                fileInfo.ObjType = type;
                fileInfo.ReadType = readType;

                FileTable.Add(fileInfo.Name, fileInfo);
                if (readType == 1) 
                    continue;

                var objInfo = new MyObjInfo();
                objInfo.Name = file.Name;
                objInfo.Obj = tempObj;
                objInfo.IsPlugin = false;
                
                ObjTable.Add(objInfo.Name, objInfo);
                Console.WriteLine($"已经读文件{file.FullName}");
            }

        }
    }
}
