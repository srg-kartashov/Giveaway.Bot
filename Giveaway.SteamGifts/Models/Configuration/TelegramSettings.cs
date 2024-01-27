using Newtonsoft.Json;

namespace Giveaway.SteamGifts.Models
{
    internal class TelegramSettings
    {
        public string? BotToken { get; set; }
        public string? ChatId { get; set; }
        public bool Preview { get; set; }
    }
}
