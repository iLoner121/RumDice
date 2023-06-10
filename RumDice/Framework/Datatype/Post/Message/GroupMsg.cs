using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RumDice.Framework {
    public class GroupMsg : BaseMsg {
        /// <summary>
        /// 群号
        /// </summary>
        public long GroupID { get; set; }
        /// <summary>
        /// 群名片
        /// </summary>
        public string? Card { get; set; }
        /// <summary>
        /// 地区
        /// </summary>
        public string? Area { get; set; }
        /// <summary>
        /// 成员等级
        /// </summary>
        public string? Level { get; set; }
        /// <summary>
        /// 角色：owner群主 admin管理员 member群员
        /// </summary>
        public string? Role { get; set; }
        /// <summary>
        /// 专属头衔
        /// </summary>
        public string? Title { get; set; }
        /// <summary>
        /// 匿名ID
        /// </summary>
        public long? AnonymousID { get; set; }
        /// <summary>
        /// 匿名名称
        /// </summary>
        public string? AnonymousName { get; set; }
        /// <summary>
        /// 匿名用户flag
        /// </summary>
        public string? Flag { get; set; }


    }
}
