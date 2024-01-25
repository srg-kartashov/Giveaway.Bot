using Newtonsoft.Json;

namespace Giveaway.SteamGifts.Models
{
    internal class Configuration
    {
        [JsonProperty(Required = Required.Always)]
        public string DriverProfilePath { get; set; } = null!;
        [JsonProperty(Required = Required.Always)]
        public TelegramSettings Telegram { get; set; } = null!;
        [JsonProperty(Required = Required.Always)]
        public FilterSettings Filter { get; set; } = null!;
    }
}
