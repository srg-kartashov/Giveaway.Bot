using Giveaway.SteamGifts.Models;

namespace Giveaway.SteamGifts.Filters
{
    internal class CollectionFilterHandler : BaseFilterHandler
    {
        public bool JoinCollections { get; }

        public CollectionFilterHandler(bool joinCollections)
        {
            JoinCollections = joinCollections;
        }

        public override bool Filter(GiveawayData game)
        {
            return game.IsCollection && JoinCollections;
        }
    }
}