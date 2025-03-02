
namespace RumDice.Framework{

    public class UserCenter : IUserCenter
    {

        private string GetPrimary(){
            Random random = new();
            string res = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString().Substring(7);
            for (int i=1; i<=5; i++){
                res += random.Next(9);
            }
            return res;
        }
        public User GetUser(string userID, BotType botType)
        {
            throw new NotImplementedException();
        }

        public User GetUser(string pramaryKey)
        {
            throw new NotImplementedException();
        }

        public User NewUser(Post post, UPermissionType userPermissionType = UPermissionType.nomal)
        {
            throw new NotImplementedException();
        }

        public User NewUser(Dictionary<BotType, string> UserID, UPermissionType userPermissionType = UPermissionType.nomal)
        {
            Dictionary<string, string> dict = new();
            foreach (var i in UserID){
                dict[i.Key.ToString()] = i.Value;
            }
            User res = NewUser(dict, userPermissionType);
            return res;
        }

        public User NewUser(Dictionary<string, string> UserID, UPermissionType userPermissionType = UPermissionType.nomal)
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