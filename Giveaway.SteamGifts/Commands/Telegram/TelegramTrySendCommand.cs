using Giveaway.SteamGifts.Models;
using Giveaway.SteamGifts.Services;

namespace Giveaway.SteamGifts.Commands
{
    internal class TelegramTrySendCommand : BaseCommand
    {
        public TelegramService TelegramService { get; private set; }

        public TelegramTrySendCommand(Configuration configuration) : base(configuration)
        {
            TelegramService = new TelegramService(configuration.Telegram.BotToken, configuration.Telegram.ChatId);
        }

        public override void Execute()
        {
            TelegramService.SendMessage("Hello World!");
        }
    }
}
