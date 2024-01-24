using AutoMapper;

using Giveaway.Steam.Converters;
using Giveaway.Steam.Models;
using Giveaway.Steam.Models.DTOs;


namespace Giveaway.Steam
{
    internal class MappingProfile:Profile
    {
        public MappingProfile()
        {
            CreateMap<SteamGameInfoDto, SteamGameInfo>().ConvertUsing<GameInfoDtoToSteamGameInfoConverter>();
        }
    }
}
