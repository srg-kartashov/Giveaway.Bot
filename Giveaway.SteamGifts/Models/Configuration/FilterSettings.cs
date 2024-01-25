using Newtonsoft.Json;
namespace Giveaway.SteamGifts.Models
{
    internal class FilterSettings
    {
        public bool EnterCollection { get; set; }
        public int MinReviewsForEnter { get; set; }
        public int MinRatingForEnter { get; set; }

    }
}
