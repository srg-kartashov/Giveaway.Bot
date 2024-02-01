using Giveaway.SteamGifts.Models;

namespace Giveaway.SteamGifts.Filters
{
    internal abstract class BaseFilterHandler
    {
        public abstract bool Filter(GiveawayData game);
    }
}