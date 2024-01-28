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

        public TelegramService(string? botToken, string? chatId)
        {
            BotToken = botToken ?? string.Empty;
            ChatId = chatId ?? string.Empty;
            TelegramClient = new TelegramBotClient(botToken ?? string.Empty);
        }

        public void SendMessage(string message, bool enablePreview = false)
        {
            if (string.IsNullOrEmpty(ChatId) || string.IsNullOrEmpty(BotToken) )
            {
                Logger.Warn("Не могу отправить сообщение в Telegram. Ключи отсутствуют");
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
