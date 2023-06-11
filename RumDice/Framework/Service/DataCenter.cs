using CSScriptLib;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RumDice.Framework.Datatype;
using System;
using System.Collections.Concurrent;
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
        public static IDataCenter? Instance { get; private set; }
        readonly IServiceManager _serviceManager;
        readonly IRumLogger _logger;
        string _root=null;

        public AppSetting AppSetting { get; private set;} = new();
        public DataCenter(IServiceManager serviceManager, IRumLogger logger) {
            Instance = this;
            _serviceManager = serviceManager;
            _logger = logger;
        }
        public ConcurrentDictionary<string, MyObjInfo> ObjTable { get; }=new();

        public Dictionary<string, MyFileInfo> FileTable { get; }=new();

        #region private method
        /// <summary>
        /// 尝试从硬盘读取并添加到内存
        /// </summary>
        /// <param name="path">文件的相对路径</param>
        /// <param name="isNewObj">是否为新加入的对象（不存在于FileTable之中）</param>
        /// <returns>是否添加成功</returns>
        bool TryAddObj(string path, bool isNewObj = false) {
            int n;
            Type t;
            string fullpath = _root + (path.StartsWith("\\") ? "" : "\\") + path;
            // 说明该文件在本地也不存在
            var obj = ReadFile(fullpath, out n, out t); ;
            if (obj == null) {
                return false;
            }
            // 如果是新对象则添加到FileTable
            if (isNewObj) {
                var fileInfo = new MyFileInfo();
                fileInfo.Name=path;
                fileInfo.Location = fullpath;
                fileInfo.ReadType = n;
                fileInfo.ObjType = t;
                FileTable.Add(path, fileInfo);
            }
            // 添加到ObjTable
            var objInfo = new MyObjInfo();
            objInfo.Name = path;
            objInfo.IsPlugin = false;
            objInfo.Obj = obj;

            if (!ObjTable.TryAdd(path, objInfo))
                return false;
            return true;
        }

        void HandleUnknownFile(string jsonString,Type type,Action<object> action) {
            var tempObj = Activator.CreateInstance(type);
            tempObj= JsonConvert.DeserializeObject(jsonString, type);
            if (tempObj == null)
                return;
            action(tempObj);
        }

        /// <summary>
        /// 从硬盘读取文件
        /// </summary>
        /// <param name="path">文件绝对相对路径</param>
        /// <param name="readType">返回：读取类型</param>
        /// <param name="objType">返回：文件类型string</param>
        /// <returns>序列化后的文件对象</returns>
        object ReadFile(string path, out int readType, out Type objType,Type? t = null,Action<object>? action=null) {
            readType = -1;
            objType = null;
            if (!File.Exists(path)) {
                _logger.Warn("DataCenter", $"磁盘内不存在此目标文件：{path}");
                return null;
            }
            // 读取文件
            string JsonString=null;
            try {
                JsonString = File.ReadAllText(path, Encoding.UTF8);
            }
            catch(Exception ex) {
                _logger.Error(ex, "读取文件错误");
                return null;
            }
           
            JObject jObj = JObject.Parse(JsonString);
            try {
                // 如果json中没有标明类型则返回null
                if (!jObj.ContainsKey("fileType")) {
                    if(t != null&&action!=null) {
                        HandleUnknownFile(JsonString, t, action);
                    }
                    return null;
                }
                string typeName = jObj["fileType"].ToString();
                // 识别读取类型
                if (jObj.ContainsKey("readType")) {
                    int.TryParse(jObj["readType"].ToString(), out readType);
                }
                // 默认值为1
                if (readType == -1)
                    readType = 1;

                // 反序列化获取对象
                Type type;
                var tempObj = _serviceManager.GetStruct(typeName, out type);
                if(tempObj == null) { 
                    if(t != null && action!=null) {  
                        HandleUnknownFile(JsonString, t, action);
                    }
                    _logger.Warn("DataCenter", "无法以已声明类型反序列化该文件");
                    return null;
                }
                tempObj = JsonConvert.DeserializeObject(JsonString,type);

                objType = type;
                return tempObj;
            }
            catch(Exception ex) {
                _logger.Warn(ex, $"从硬盘中直接读取文件失败：{path}");
                return null;
            }
        }

        bool TryAddNewFile(string fullName,Type? t=null,Action<object>? action =null) {
            // 读取文件对象
            int readType = -1;
            Type type = null;
            var tempObj = ReadFile(fullName, out readType, out type , t, action);
            // 读取失败
            if (tempObj == null)
                return false;

            string s = fullName;
            string name;
            if (s.IndexOf(_root, 0, _root.Length, StringComparison.Ordinal) > -1) {
                name = s.Substring(_root.Length);
            } else {
                name = s;
            }


            if (FileTable.ContainsKey(name))
                return false;

            // 添加到FileTable
            var fileInfo = new MyFileInfo();
            fileInfo.Location = fullName;
            fileInfo.Name = name;
            fileInfo.ObjType = type;
            fileInfo.ReadType = readType;

            FileTable.Add(fileInfo.Name, fileInfo);
            _logger.Debug("DataCenter", $"已经识别文件{name}");

            if (readType == 1)
                return true;

            if (ObjTable.ContainsKey(name)) 
                return false;

            // 添加到ObjTable
            var objInfo = new MyObjInfo();
            objInfo.Name = name;
            objInfo.Obj = tempObj;
            objInfo.IsPlugin = false;

            ObjTable.TryAdd(objInfo.Name, objInfo);
            _logger.Debug("DataCenter", $"已将文件：{objInfo.Name} 读取至内存，文件类型为{type.Name}");
            return true;
        }

        #endregion

        public object GetObj(string path) {
            // 为路径结尾添加.json
            path = (path.EndsWith(".json") ? path : (path + ".json"));
            // 如果文件不存在
            if (!FileTable.ContainsKey(path)) {
                _logger.Warn("DataCenter", "未读取过该文件，尝试重新读取");
                string fullPath = _root + (path.StartsWith("\\") ? "" : "\\") + path;
                if(!TryAddNewFile(fullPath))
                    return null;
            }
            // 如果文件类型为3：永久存在于内存
            if (FileTable[path].ReadType == 3) {
                // 如果内存中不存在则读取新对象
                if (!ObjTable.ContainsKey(path)) {
                    if (!TryAddObj(path))
                        return null;
                }
                return ObjTable[path].Obj;
            }
            // 如果文件类型为2：临时存在于内存
            if (FileTable[path].ReadType == 2) {
                // 如果内存中不存在则读取新对象
                if (!ObjTable.ContainsKey(path))
                    if (!TryAddObj(path))
                        return null;
                // 如果内存中存在则更新使用次数
                var tempObj = ObjTable[path];
                int tempUsed = ++ObjTable[path].UsedTime;
                int maxUsed = tempObj.MaxTime;
                if (tempUsed > maxUsed) {
                    ObjTable.Remove(path,out MyObjInfo? obj);
                }
                return tempObj.Obj;
            }
            // 如果文件类型为1：不在内存中
            if (FileTable[path].ReadType==1) {
                // 读取并返回
                int n;
                Type t;
                var obj = ReadFile(FileTable[path].Location, out n, out t);
                if(obj == null) {
                    return null;
                }
                return obj;
            }
            return null;
        }

        public bool TryGetObj(string path, out object obj) {
            obj = GetObj(path);
            if (obj == null)
                return false;
            return true;
        }

        public List<object> GetByType(string type) {
            List<object> res = new();
            foreach(var fileInfo in FileTable.Values) {
                if (fileInfo.ObjType.Name == type) {
                    var obj = GetObj(fileInfo.Name);
                    if(obj!=null)
                        res.Add(obj);
                }
            }
            return res;
        }

        public List<object> GetByPath(string path) {
            // 获取全路径
            string fullPath=_root+(path.StartsWith("\\")?"":"\\")+path;
            List<object> res = new();
            foreach (var fileInfo in FileTable.Values) {
                if (fileInfo.Location.StartsWith(fullPath)) {
                    var obj = GetObj(fileInfo.Name);
                    if (obj != null)
                        res.Add(obj);
                }
            }
            return res;
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public bool SaveFile(object obj, Type type,string path, int readType = -1) {
            // 为路径结尾添加.json
            path = (path.EndsWith(".json") ? path : ( path + ".json"));
            // 如果文件已被读取过
            if (FileTable.ContainsKey(path)) {
                // 文件类型不匹配
                if (FileTable[path].ObjType != type) {
                    return false;
                }
                if (readType == -1)
                    readType = FileTable[path].ReadType;
            // 如果文件未被读取过
            } else {
                // 默认读取类型为3
                if (readType == -1)
                    readType = 1;

                // 添加到FileTable
                var fileInfo = new MyFileInfo();
                fileInfo.Name = path;
                fileInfo.ReadType = readType;
                fileInfo.Location = _root + (path.StartsWith("\\") ? "" : "\\") + path;
                fileInfo.ObjType = type;
                FileTable.Add(path, fileInfo);
            }
            // 如果内存中已经存在实例
            if(ObjTable.ContainsKey(path)) {
                // 更新实例
                var objInfo = ObjTable[path];
                objInfo.Obj = obj;
                ObjTable.Remove(path,out MyObjInfo? o);
                if(readType != 1) {
                    if (!ObjTable.TryAdd(path, objInfo))
                        return false;
                }
            }
            try {
                // 序列化
                string jsonString = JsonConvert.SerializeObject(obj);

                // 添加必要字段
                JObject jObj = JObject.Parse(jsonString);
                if (!jObj.ContainsKey("readType")) {
                    jObj.Add(new JProperty("readType",readType));
                }
                if (!jObj.ContainsKey("fileType")) {
                    jObj.Add(new JProperty("fileType", type.Name));
                }
                jsonString= JsonConvert.SerializeObject(jObj);

                if (File.Exists(FileTable[path].Location)) {
                    File.Delete(FileTable[path].Location);
                }

                // 写入文件
                _logger.Debug("DataCenter","开始向硬盘写入文件");
                FileStream fs = new FileStream(FileTable[path].Location, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);
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

        [MethodImpl(MethodImplOptions.Synchronized)]
        public bool SaveFile<T>(T obj,string path,int readType=-1) {
            return SaveFile(obj, typeof(T), path, readType);
        }

        public void Initialize(AppSetting appSetting,string RootDic) {
            AppSetting = appSetting;
            _root = RootDic + AppSetting.FileConfig.RepositoryRoot;
        }

        public async ValueTask ScanFile() {
            if (!Directory.Exists(_root)) {
                Directory.CreateDirectory(_root);
            }
            // 获取所有.json文件
            DirectoryInfo dir = new DirectoryInfo(_root);
            var files = dir.GetFiles("*.json", System.IO.SearchOption.AllDirectories);
            foreach ( var file in files ) {
                TryAddNewFile(file.FullName);
            }
            _logger.Debug("DataCenter", "本地文件扫描完成");
        }

        public async ValueTask ScanFile(string path, Type type, Action<object>? action) {
            if(!Directory.Exists(_root+path)) { 
                Directory.CreateDirectory(_root+path);
            }
            DirectoryInfo dir = new DirectoryInfo(_root+path);
            var files = dir.GetFiles("*.json",System.IO.SearchOption.AllDirectories);
            foreach (var file in files) {
                TryAddNewFile(file.FullName,type,action);
            }
            _logger.Debug("DataCenter", "本地文件扫描完成");
        }
    }
}
