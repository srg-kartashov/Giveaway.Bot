using Giveaway.SteamGifts.Models;
using Giveaway.SteamGifts.Services;

using NLog;

namespace Giveaway.SteamGifts.Commands
{
    internal class TelegramTrySendCommand : BaseCommand
    {
        public string BotToken { get; set; }
        public string ChatId { get; set; }

        public ILogger Logger => LogManager.GetCurrentClassLogger();

        public TelegramTrySendCommand(Configuration configuration) : base(configuration)
        {
            BotToken = configuration.Telegram.BotToken ?? string.Empty;
            ChatId = configuration.Telegram.ChatId ?? string.Empty;
        }

        public override void Execute()
        {
            try
            {
                if (string.IsNullOrEmpty(ChatId) || string.IsNullOrEmpty(BotToken))
                {
                    Console.WriteLine("Ключи для отправки сообщения отсутствуют");
                }
                var telegramService = new TelegramService(BotToken, ChatId);
                telegramService.SendMessage("Hello World!");
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                Console.WriteLine("Ошибка во время попытки отправки сообщения. Проверьте BotToken и ChatId");
            }
            finally
            {
                Console.WriteLine("Press any key to continue...");
                Console.ReadLine();
            }
        }
    }
}
