using AutoMapper;

using Giveaway.Steam.Models;
using Giveaway.Steam.Models.DTOs;

namespace Giveaway.Steam.Converters
{
    internal class GameInfoDtoToSteamGameInfoConverter : ITypeConverter<SteamGameInfoDto, SteamGameInfo>
    {
        public SteamGameInfo Convert(SteamGameInfoDto source, SteamGameInfo destination, ResolutionContext context)
        {
            var steamGameInfo = new SteamGameInfo();
            steamGameInfo.TotalReviews = source.QuerySummary.TotalReviews;
            double rating;
            if (source.QuerySummary.TotalReviews == 0 || source.QuerySummary.TotalPositive == 0)
            {
                rating = 0;
            }
            else
            {
                rating = source.QuerySummary.TotalPositive * 1.0 / source.QuerySummary.TotalReviews * 100;
            }
            steamGameInfo.Raiting = System.Convert.ToInt32(rating);
            return steamGameInfo;
        }
    }
}
