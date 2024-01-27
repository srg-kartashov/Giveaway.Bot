using Newtonsoft.Json;

namespace Giveaway.SteamGifts.Models
{
    internal class Configuration
    {
        [JsonProperty(Required = Required.Always)]
        public bool StopAfterPointsEnded { get; set; }
        [JsonProperty(Required = Required.Always)]
        public string DriverProfilePath { get; set; } = null!;
        [JsonProperty(Required = Required.Always)]
        public TelegramSettings Telegram { get; set; } = null!;
        [JsonProperty(Required = Required.Always)]
        public bool EnterCollections { get; set; }
        [JsonProperty(Required = Required.Always)]
        public FilterSettings [] EnterFilters { get; set; } = null!;
        [JsonProperty(Required = Required.Always)]
        public FilterSettings[] HideFilters { get; set; } = null!;
    }
}
