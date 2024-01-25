using Giveaway.SteamGifts.Models;

using NLog;

using Telegram.Bot;
using Telegram.Bot.Types.Enums;

namespace Giveaway.SteamGifts.Services
{
    internal class TelegramService
    {
        public ILogger Logger => LogManager.GetCurrentClassLogger();
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
                Logger.Warn("Telegram ключи отсутствуют");
            }
            try
            {
                TelegramClient.SendTextMessageAsync(ChatId, message,
                    parseMode: ParseMode.Html, disableWebPagePreview: !enablePreview).Wait();
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Ошибка во время отправки сообщения");
            }
        }
    }
}
