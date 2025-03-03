
namespace RumDice.Framework{

    public class UserCenter : IUserCenter
    {

        private string serviceName = "IUserCenter";
        private string local = "\\System\\User";
        private UserList? userList;
        private UserIndex? userIndex;

        private string GetPrimary(){
            Random random = new();
            string res = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString().Substring(5);
            for (int i=1; i<=5; i++){
                res += random.Next(9);
            }
            return res;
        }
        public User GetUser(string userID, BotType botType)
        {
            if (DataCenter.Instance.TryGetObj(local+"\\UserIndex.json", out Object obj)){
                userIndex = (UserIndex)obj;
            }
            else{
                return null;
            }
            if (userIndex.table[botType.ToString()].ContainsKey(userID) == false){
                return null;
            }
            return GetUser(userIndex.table[botType.ToString()][userID]);
        }

        public User GetUser(string pramaryKey)
        {
            if (DataCenter.Instance.TryGetObj(local+"\\UserList.json", out Object obj)){
                userList = (UserList)obj;
            }
            else{
                return null;
            }
            if (userList.table.ContainsKey(pramaryKey) != true){
                return null;
            }
            return userList.table[pramaryKey];
        }

        public User NewUser(Post post, UPermissionType userPermissionType = UPermissionType.Normal)
        {
            if (post is not BaseMsg){
                RumLogger.Instance.Debug(serviceName, "NewUser:传入了错误的Post变量");
                return null;
            }
            BaseMsg baseMsg = (BaseMsg)post;
            User res = NewUser(new Dictionary<string, string>{{baseMsg.BotType.ToString(), baseMsg.UserID.ToString()}});
            return res;
        }

        public User NewUser(Dictionary<BotType, string> UserID, UPermissionType userPermissionType = UPermissionType.Normal)
        {
            Dictionary<string, string> dict = new();
            foreach (var i in UserID){
                dict[i.Key.ToString()] = i.Value;
            }
            User res = NewUser(dict, userPermissionType);
            return res;
        }

        public User NewUser(Dictionary<string, string> UserID, UPermissionType userPermissionType = UPermissionType.Normal)
        {
            User res = new();
            foreach (BotType i in Enum.GetValues(typeof(BotType))){
                res.UserID.Add(i.ToString(), "0");
            }
            foreach (var i in UserID){
                if (res.UserID.ContainsKey(i.Key)){
                    res.UserID[i.Key] = i.Value;
                }
            }
            res.Primary = GetPrimary();
            res.Permission = $"{(int)userPermissionType}";
            return res;
        }
    }
}