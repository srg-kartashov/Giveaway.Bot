using Giveaway.SteamGifts.Models;

using NLog;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Giveaway.SteamGifts.Commands
{
    internal class TelegramShowKeysCommand : BaseCommand
    {
        public TelegramShowKeysCommand(Configuration config) : base(config)
        {
        }

        public override void Execute()
        {
            Console.WriteLine("Telegram BotToken: " + Configuration.Telegram.BotToken);
            Console.WriteLine("Telegram ChatID: " + Configuration.Telegram.ChatId);
            Console.WriteLine("Press any key to continue...");
            Console.ReadLine();
        }
    }
}
