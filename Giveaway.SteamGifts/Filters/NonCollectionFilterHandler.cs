using Giveaway.SteamGifts.Models;

namespace Giveaway.SteamGifts.Filters
{
    internal class NonCollectionFilterHandler : BaseFilterHandler
    {
        public NonCollectionFilterHandler()
        {
        }

        public override bool Filter(GiveawayData game)
        {
            return !game.IsCollection;
        }
    }
}