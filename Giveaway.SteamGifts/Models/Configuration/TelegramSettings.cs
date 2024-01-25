using Newtonsoft.Json;

namespace Giveaway.SteamGifts.Models
{
    internal class TelegramSettings
    {
        public string? BotToken { get; set; }
        public string? ChatId { get; set; }
        public bool TelegramSendWithPreview { get; set; }

        [JsonIgnore]
        public bool Enabled => !string.IsNullOrWhiteSpace(BotToken) && !string.IsNullOrWhiteSpace(ChatId);
    }
}
