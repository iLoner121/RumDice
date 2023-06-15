using RumDice.Framework.Datatype;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace RumDice.Framework {
    /// <summary>
    /// 本系统的数据库
    /// </summary>
    [MyClass]
    public interface IDataCenter {
        /// <summary>
        /// 系统设置
        /// </summary>
        AppSetting AppSetting { get; }
        /// <summary>
        /// 包含了已被解析的json对象
        /// </summary>
        ConcurrentDictionary<string, MyObjInfo> ObjTable { get; }
        /// <summary>
        /// 所有已读取到文件的信息
        /// </summary>
        Dictionary<string, MyFileInfo> FileTable { get; }

        /// <summary>
        /// 获取文件
        /// </summary>
        /// <param name="path">文件的相对路径</param>
        /// <returns>取回的obj（无对应文件则取回null）</returns>
        Object GetObj(string path);
        /// <summary>
        /// 获取文件
        /// </summary>
        /// <param name="path">文件的相对路径</param>
        /// <param name="obj">取回的对象（无对应文件则取回null）</param>
        /// <returns>是否成功取回obj</returns>
        bool TryGetObj(string path,out Object obj);


        /// <summary>
        /// 通过类别获取
        /// </summary>
        /// <param name="type">该json对应的类别</param>
        /// <returns></returns>
        List<Object> GetObjByType(string type);

        /// <summary>
        /// 通过相对路径获取
        /// </summary>
        /// <param name="path">相对路径（该路径下的所有子文件都会被传回）</param>
        /// <returns></returns>
        List<Object> GetObjByPath(string path);

        /// <summary>
        /// 保存文件（包括更新内存中的缓存数据）
        /// </summary>
        /// <param name="obj">要保存的数据对象</param>
        /// <param name="type">该数据对象的类型（必须被声明为MyStruct）</param>
        /// <param name="path">保存路径（当保存到根目录下时可以缺省）（不能和下一项同时缺省）</param>
        /// <param name="name">文件名（当路径结尾包含了文件名时可以缺省）（不能和上一项同时缺省）</param>
        /// <param name="ReadType">读取类型（保存已有文件默认为原类型，保存新文件默认为1）</param>
        /// <returns>是否保存成功</returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        bool SaveFile(Object obj,Type type,string? path=null,string? name=null,int ReadType=-1);


        /// <summary>
        /// 保存文件
        /// </summary>
        /// <typeparam name="T">文件对象类型</typeparam>
        /// <param name="obj">要保存的数据对象</param>
        /// <param name="path">保存路径（当保存到根目录下时可以缺省）（不能和下一项同时缺省）</param>
        /// <param name="name">文件名（当路径结尾包含了文件名时可以缺省）（不能和上一项同时缺省）</param>
        /// <param name="ReadType">读取类型（保存已有文件默认为原类型，保存新文件默认为1）</param>
        /// <returns>是否保存成功</returns>
        bool SaveFile<T>(T obj,string? path = null,string? name=null,int ReadType=-1);

        /// <summary>
        /// 扫描并读取文件根目录下所有系统中声明过的文件
        /// </summary>
        /// <returns></returns>
        ValueTask ScanAll();

        void Initialize(AppSetting appSetting,string RootDic);

        /// <summary>
        /// 将目录下的文件尝试以某类型读取(如果某文件已经被读取过，则跳过该文件)
        /// </summary>
        /// <param name="path">文件路径</param>
        /// <param name="type">文件类型</param>
        /// <param name="action">读取到某个文件后将执行的操作（为空则执行SaveFile）</param>
        /// <returns></returns>
        ValueTask CustomScan(string path,Type type,Action<object>? action);

        List<object> CustomScan(string path, Type type);

        ValueTask CustomRead(string path,Type type,Action<object>? action);

        object CustomRead(string path, Type type);

        /// <summary>
        /// 读取txt文件
        /// </summary>
        /// <param name="path">文件路径（含文件名）</param>
        /// <returns></returns>
        string ReadTxtFile(string path);
        /// <summary>
        /// 保存txt文件
        /// </summary>
        /// <param name="content">txt文件内容</param>
        /// <param name="path">路径（含文件名）</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        bool SaveTxtFile(string content,string? path = null,string? name=null);

    }
}
