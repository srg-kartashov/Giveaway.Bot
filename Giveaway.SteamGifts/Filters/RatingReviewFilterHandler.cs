using Giveaway.SteamGifts.Models;

namespace Giveaway.SteamGifts.Filters
{
    internal class RatingReviewFilterHandler : BaseFilterHandler
    {
        public FilterSettings[] FilterSettings { get; }

        public RatingReviewFilterHandler(FilterSettings[] filterSettings)
        {
            FilterSettings = filterSettings;
        }

        public override bool Filter(GameGiveaway game)
        {
            foreach (var filterSetting in FilterSettings)
            {
                if (game.Raiting >= filterSetting.RatingFrom &&
                    game.Raiting <= filterSetting.RatingTo &&
                    game.Reviews >= filterSetting.ReviewsFrom &&
                    game.Reviews <= filterSetting.ReviewsTo)
                    return true;
            }
            return false;
        }
    }
}