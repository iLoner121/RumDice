namespace RumDice.Framework{

    [MyClass]
    public interface IUserCenter{
        /// <summary>
        /// 根据用户平台账号获取对应用户
        /// </summary>
        /// <param name="userID">用户平台账号</param>
        /// <param name="botType">用户平台类型</param>
        /// <returns>返回对应User，无对应用户或获取失败时返回null</returns>
        public User GetUser(string userID, BotType botType);

        /// <summary>
        /// 根据传入的用户唯一索引获取用户
        /// </summary>
        /// <param name="primaryKey">用户唯一索引</param>
        /// <returns>返回对应User，无对应用户或获取失败时返回null</returns>
        public User GetUser(string primaryKey);

        /// <summary>
        /// 根据发信包快速创建对应用户
        /// </summary>
        /// <param name="post">收到的发信包</param>
        /// <param name="userPermissionType">新用户权限</param>
        /// <returns>返回创建的User，创建失败时返回null</returns>
        public User NewUser(Post post, UPermissionType userPermissionType=UPermissionType.Normal);

        /// <summary>
        /// 通过传入的用户各账号创建账户
        /// </summary>
        /// <param name="UserID">用户账号字典<用户平台类型，账号></param>
        /// <param name="userPermissionType">新用户权限</param>
        /// <returns>返回对应的User，创建失败时返回null</returns>
        public User NewUser(Dictionary<BotType, string> UserID, UPermissionType userPermissionType=UPermissionType.Normal);

        /// <summary>
        /// 通过传入的用户各账号创建账户
        /// </summary>
        /// <param name="UserID">用户账号字典<用户平台类型，账号></param>
        /// <param name="userPermissionType">新用户权限</param>
        /// <returns>返回对应的User，创建失败时返回null</returns>
        public User NewUser(Dictionary<string, string> UserID, UPermissionType userPermissionType=UPermissionType.Normal);

        /// <summary>
        /// 加入新用户
        /// </summary>
        /// <param name="user">新用户</param>
        /// <returns></returns>
        public bool Add(User user);

        [Listen(AllType.Start)]
        public void Initialize(Post post);
    }
}