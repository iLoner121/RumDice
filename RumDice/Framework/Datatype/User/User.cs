namespace RumDice.Framework{

    public class User{
        public Dictionary<string, string> UserID{get; set;} = new();
        public Dictionary<string, string> Info{get; set;} = new();
        public string Primary = "";
        public string Permission = "3";
    }
}