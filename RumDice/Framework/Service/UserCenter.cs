
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
            User? user = null;
            if (DataCenter.Instance.TryGetObj(local+"\\UserIndex.json", out Object obj)){
                userIndex = (UserIndex)obj;
            }
            else{
                RumLogger.Instance.Debug(serviceName, "UserIndex异常");
            }
            if (userIndex != null && userIndex.table[botType.ToString()].ContainsKey(userID)){
                user = GetUser(userIndex.table[botType.ToString()][userID]);
            }
            userIndex = null;
            return user;
        }

        public User GetUser(string primaryKey)
        {
            User? user = null;
            if (DataCenter.Instance.TryGetObj(local+"\\UserList.json", out Object obj)){
                userList = (UserList)obj;
            }
            else{
                RumLogger.Instance.Debug(serviceName, "UserList异常");
            }
            if (userList != null && userList.table.ContainsKey(primaryKey)){
                user = userList.table[primaryKey];
            }
            userList = null;
            return user;
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

        public bool Add(User user)
        {
            if (DataCenter.Instance.TryGetObj(local+"\\UserIndex.json", out Object obj1)){
                userIndex = (UserIndex)obj1;
            }
            else{
                RumLogger.Instance.Debug(serviceName, "UserIndex异常");
            }
            if (DataCenter.Instance.TryGetObj(local+"\\UserList.json", out Object obj2)){
                userList = (UserList)obj2;
            }
            else{
                RumLogger.Instance.Debug(serviceName, "UserList异常");
            }
            if (userIndex == null || userList == null){
                userIndex = null;
                userList = null;
                return false;
            }
            if (userList.table.ContainsKey(user.Primary)){
                RumLogger.Instance.Debug(serviceName, "添加了重复用户");
                userIndex = null;
                userList = null;
                return false;
            }
            try{
                userList.table.Add(user.Primary, user);
                foreach (var i in user.UserID){
                    if (userIndex.table.ContainsKey(i.Key)){
                        userIndex.table[i.Key][i.Value] = user.Primary;
                    }
                }
                DataCenter.Instance.SaveFile<UserList>(userList, local+"\\UserList.json", ReadType:3);
                DataCenter.Instance.SaveFile<UserIndex>(userIndex, local+"\\UserIndex.json", ReadType:3);
                userIndex = null;
                userList = null;
                return true;
            }
            catch (Exception ex){
                if (userList.table.ContainsKey(user.Primary)){
                    userList.table.Remove(user.Primary);
                }
                foreach (var i in user.UserID){
                    if (userIndex.table.ContainsKey(i.Key)){
                        if (userIndex.table[i.Key].ContainsKey(i.Value)){
                            userIndex.table[i.Key].Remove(i.Value);
                        }
                    }
                }
                RumLogger.Instance.Debug(serviceName, ex.ToString());
                userIndex = null;
                userList = null;
                return false;
            }
        }

        public bool Updata(string primaryKey, Dictionary<string, string>? info=null, Dictionary<string, string>? UserID=null)
        {
            if (DataCenter.Instance.TryGetObj(local+"\\UserIndex.json", out Object obj1)){
                userIndex = (UserIndex)obj1;
            }
            else{
                RumLogger.Instance.Debug(serviceName, "UserIndex异常");
                return false;
            }
            if (DataCenter.Instance.TryGetObj(local+"\\UserList.json", out Object obj2)){
                userList = (UserList)obj2;
            }
            else{
                RumLogger.Instance.Debug(serviceName, "UserList异常");
                return false;
            }
            if (userList.table.ContainsKey(primaryKey) == false){
                return false;
            }
            if (info != null){
                foreach (var i in info){
                    userList.table[primaryKey].Info[i.Key] = i.Value;
                }
            }
            if (UserID != null){
                foreach (var i in UserID){
                    if (userList.table[primaryKey].UserID.ContainsKey(i.Key)){
                        if (userList.table[primaryKey].UserID[i.Key] == "0"){
                            userList.table[primaryKey].UserID[i.Key] = i.Value;
                        }
                        else{
                            string curID = userList.table[primaryKey].UserID[i.Key];
                            userIndex.table[i.Key].Remove(curID);
                        }
                        userIndex.table[i.Key].Add(i.Key, i.Value);
                    }
                }
                DataCenter.Instance.SaveFile<UserIndex>(userIndex, local+"\\UserIndex.json", ReadType:3);
            }
            DataCenter.Instance.SaveFile<UserList>(userList, local+"\\UserList.json", ReadType:3);
            userList = null;
            userList = null;
            return true;
        }

        public void Initialize(Post post)
        {
            userList = new UserList();
            userIndex = new UserIndex();
            if (DataCenter.Instance.TryGetObj(local+"\\UserIndex.json", out Object obj1)){
                userIndex = (UserIndex)obj1;
            }
            else{
                RumLogger.Instance.Debug(serviceName, "UserIndex异常");
            }
            if (DataCenter.Instance.TryGetObj(local+"\\UserList.json", out Object obj2)){
                userList = (UserList)obj2;
            }
            else{
                RumLogger.Instance.Debug(serviceName, "UserList异常");
            }
            userList.remark = "用户表";
            userIndex.remark = "用户索引";
            foreach (BotType i in Enum.GetValues(typeof(BotType))){
                if (userIndex.table.ContainsKey(i.ToString()) == false){
                    userIndex.table.Add(i.ToString(), new Dictionary<string, string>());
                }
            }
            DataCenter.Instance.SaveFile<UserList>(userList, local+"\\UserList.json", ReadType:3);
            DataCenter.Instance.SaveFile<UserIndex>(userIndex, local+"\\UserIndex.json", ReadType:3);
            userList = null;
            userIndex = null;
        }
    }
}