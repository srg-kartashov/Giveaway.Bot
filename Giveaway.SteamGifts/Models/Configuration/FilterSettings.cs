using Newtonsoft.Json;
namespace Giveaway.SteamGifts.Models
{
    internal class FilterSettings
    {
        public bool TelegramSendAboutNotEnteredGames { get; set; }
        public bool NeedHide { get; set; }
        public int MinReviewsForEnter { get; set; }
        public int MinRatingForEnter { get; set; }
        public bool EnterCollection { get; set; }

    }
}
