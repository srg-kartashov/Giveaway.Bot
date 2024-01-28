namespace Giveaway.SteamGifts.Models
{
    internal class UserData
    {
        public UserData()
        {
            
        }
        public UserData(string name, int level, int points)
        {
            Name = name;
            Level = level;
            Points = points;
        }
        public string Name { get; set; } = null!;
        public int Level { get; set; }
        public int Points { get; set; }
    }
}
