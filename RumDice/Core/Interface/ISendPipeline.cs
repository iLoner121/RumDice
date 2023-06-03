using RumDice.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RumDice.Core {
    /// <summary>
    /// 信息发送管道
    /// </summary>
    public interface ISendPipeline {

        /// <summary>
        /// 发送信息
        /// </summary>
        /// <param name="sends"></param>
        /// <returns></returns>
        public bool SendMsg(List<Send> sends);
    }
}
