using Giveaway.SteamGifts.Models;

using NLog;

namespace Giveaway.SteamGifts.Commands
{
    internal class TelegramShowKeysCommand : BaseCommand
    {
        public ILogger Logger => LogManager.GetCurrentClassLogger();

        public TelegramShowKeysCommand(Configuration config) : base(config)
        {
        }

        public override void Execute()
        {
            try
            {
                Console.WriteLine("Telegram BotToken: " + Configuration.Telegram.BotToken);
                Console.WriteLine("Telegram ChatID: " + Configuration.Telegram.ChatId);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                Console.WriteLine("Ошибка во время попытки показа ключей");
            }
            finally
            {
                Console.WriteLine("Press any key to continue...");
                Console.ReadLine();
            }
        }
    }
}
