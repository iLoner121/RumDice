using RumDice.Framework.Datatype;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RumDice.Framework {
    /// <summary>
    /// 本系统的数据库
    /// </summary>
    public interface IDataCenter {
        /// <summary>
        /// 包含了已被解析的json对象
        /// </summary>
        Dictionary<string, MyObjInfo> ObjTable { get; }
        /// <summary>
        /// 所有已读取到文件的信息
        /// </summary>
        Dictionary<string, MyFileInfo> FileTable { get; }

        /// <summary>
        /// 通过文件名获取
        /// </summary>
        /// <param name="name">文件的名称</param>
        /// <returns></returns>
        Object GetByName(string name);
        /// <summary>
        /// 通过类别获取
        /// </summary>
        /// <param name="type">该json对应的类别</param>
        /// <returns></returns>
        List<Object> GetByType(string type);
        /// <summary>
        /// 获取全部内建文件
        /// </summary>
        /// <returns></returns>
        List<Object> GetAllInnerFile();
        /// <summary>
        /// 获取全部插件文件
        /// </summary>
        /// <returns></returns>
        List<Object> GetAllPluginFile();
        /// <summary>
        /// 获取所有文件
        /// </summary>
        /// <returns></returns>
        List<Object> GetAll();
        /// <summary>
        /// 保存文件
        /// </summary>
        /// <param name="obj">文件对象</param>
        /// <param name="type">对象类型</param>
        /// <param name="ReadType">读取类型：3永远处于内存中 2使用十次后从内存中消除 1每次都从内存中读取</param>
        /// <param name="path"></param>
        /// <returns></returns>
        bool SaveFile(Object obj,Type type,string name,int ReadType=2);

        ValueTask ScanFile();

        void Initialize(string Root);

    }
}
