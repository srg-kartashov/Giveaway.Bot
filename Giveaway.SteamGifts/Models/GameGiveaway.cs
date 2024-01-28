using Giveaway.Steam.Models;
using Giveaway.SteamGifts.Pages.SteamGift.Elements;

namespace Giveaway.SteamGifts.Models
{
    internal class GameGiveaway
    {
        public GameGiveaway(GiveawayElement giveaway)
        {
            Name = giveaway.GameName;
            SteamUrl = giveaway.GameUrl;
            SteamGiftUrl = giveaway.GiveawayUrl;
            IsCollection = giveaway.IsCollection;
            Points = giveaway.Points;
        }
        public GameGiveaway(GiveawayElement giveaway, SteamGameInfo steamGameInfo) : this(giveaway)
        {
            Raiting = steamGameInfo.Raiting;
            Reviews = steamGameInfo.TotalReviews;
        }

        public string Name { get; set; } = null!;
        public string SteamUrl { get; set; } = null!;
        public string SteamGiftUrl { get; set; } = null!;
        public bool IsCollection { get; set; }
        public int Raiting { get; set; }
        public int Reviews { get; set; }
        public int Points { get; set; }
    }
}
