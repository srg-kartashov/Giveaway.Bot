using Giveaway.SteamGifts.Models;

using Telegram.Bot;
using Telegram.Bot.Types.Enums;

namespace Giveaway.SteamGifts.Services
{
    internal class TelegramService
    {
        public string BotToken { get; }
        public string ChatId { get; }
        private TelegramBotClient TelegramClient { get; }

        public TelegramService(string botToken, string chatId)
        {
            BotToken = botToken;
            ChatId = chatId;
            TelegramClient = new TelegramBotClient(botToken);
        }

        public void SendMessage(string message, bool enablePreview = false)
        {
            if (string.IsNullOrEmpty(ChatId) || string.IsNullOrEmpty(BotToken) )
            {
                return;
            }
            TelegramClient.SendTextMessageAsync(ChatId, message,
                parseMode: ParseMode.Html, disableWebPagePreview: !enablePreview).Wait();
        }
    }
}
