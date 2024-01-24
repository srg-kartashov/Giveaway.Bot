using Newtonsoft.Json;

namespace Giveaway.SteamGifts.Models
{
    internal class Configuration
    {
        [JsonIgnore]
        public string Path { get; set; }

        public Configuration(string path)
        {
            Path = path;
            Telegram = new TelegramSettings
            {
                BotToken = string.Empty,
                ChatId = string.Empty,
                TelegramSendWithPreview = true
            };
            Filter = new FilterSettings
            {
                MinReviewsForEnter = 500,
                MinRatingForEnter = 70,
                EnterCollection = true,
                NeedHide = false,
                TelegramSendAboutNotEnteredGames = true
            };
        }
        public TelegramSettings Telegram { get; set; }
        public FilterSettings Filter { get; set; }
        public string DriverProfilePath { get; }

        public void SaveChanges()
        {
            var json = JsonConvert.SerializeObject(this);
            File.WriteAllText(Path, json);
        }
        public void Delete()
        {
            File.Delete(Path);
        }
    }
}
