using Giveaway.SteamGifts.Models;

namespace Giveaway.SteamGifts.Filters
{
    internal class PriceFilterHandler : BaseFilterHandler
    {
        public int UserBalance { get; }
        public PriceFilterHandler(int userBalance)
        {
            UserBalance = userBalance;
        }

        public override bool Filter(GameGiveaway game)
        {
            return UserBalance > game.Points;
        }
    }
}
