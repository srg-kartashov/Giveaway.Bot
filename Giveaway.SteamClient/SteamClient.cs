using AutoMapper;
using Giveaway.Steam.Models;
using Giveaway.Steam.Models.DTOs;
using Newtonsoft.Json;

namespace Giveaway.Steam
{
    public class SteamClient
    {
        public readonly string BaseURL = "https://store.steampowered.com/appreviews/";
        private HttpClient Client { get; }
        private IMapper Mapper { get; }

        public SteamClient()
        {
            Client = new HttpClient { BaseAddress = new Uri(BaseURL) };
            var mapperConfiguration = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>());
            Mapper = new Mapper(mapperConfiguration);
        }

        public SteamGameInfo GetGameInfo(int gameId)
        {
            try
            {
                var response = Client.GetAsync($"{gameId}?json=1&language=all").Result;
                if(response.IsSuccessStatusCode)
                {
                    var content = response.Content.ReadAsStringAsync().Result;
                    var result = JsonConvert.DeserializeObject<SteamGameInfoDto>(content);
                    var steamGameInfo = Mapper.Map<SteamGameInfo>(result);
                    return steamGameInfo;
                }
                else
                {
                    return null;
                }
            }
            catch
            {
                return null;
            }
        }


    }
}
