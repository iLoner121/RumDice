using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RumDice.Framework {
    /// <summary>
    /// 上传群文件
    /// </summary>
    public class UploadGroupFileSend :OneBotSend{
        public string FilePath { get; set; }
        public string FileName { get; set; }
        /// <summary>
        /// 上传群文件发信包
        /// </summary>
        /// <param name="groupID">群组id</param>
        /// <param name="filePath">文件地址</param>
        /// <param name="fileName">文件名称</param>
        public UploadGroupFileSend(long groupID,string filePath, string fileName) {
            Action = OneBotAction.UploadGroupFile;
            GroupID = groupID;
            FilePath = filePath;
            FileName = fileName;
        }
    }
}
