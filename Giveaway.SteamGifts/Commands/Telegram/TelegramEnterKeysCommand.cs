using Giveaway.SteamGifts.Models;

using NLog;

namespace Giveaway.SteamGifts.Commands
{
    internal class TelegramEnterKeysCommand : BaseCommand
    {
        public TelegramEnterKeysCommand(Configuration configuration) : base(configuration)
        {
        }

        public override void Execute()
        {
            Console.Write("Введите Telegram BotToken: ");
            var botToken = Console.ReadLine();
            if(botToken != null)
            {
                Configuration.Telegram.BotToken = botToken;
            }
            Console.Write("Введите Telegram BotToken: ");
            var chatID = Console.ReadLine();
            if (chatID != null)
            {
                Configuration.Telegram.ChatId = chatID;
            }
            Configuration.SaveChanges();
            Console.WriteLine("Press any key to continue...");
            Console.ReadLine();

        }
    }
}
