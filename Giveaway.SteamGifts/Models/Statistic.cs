namespace Giveaway.SteamGifts.Models
{
    internal class Statistic
    {
        public int AlreadyEntered { get; set; }
        public int Skiped { get; set; }
        public int Failed { get; set; }
        public int Entered { get; set; }
        public int Hidden { get; set; }
        public int FailedHidden { get; set; }
    }
}