﻿using Giveaway.SteamGifts.Models;

using NLog;

namespace Giveaway.SteamGifts.Commands
{
    internal class StatisticCommand : BaseCommand
    {
        public ILogger Logger = LogManager.GetCurrentClassLogger();

        public StatisticCommand(Configuration configuration) : base(configuration)
        {
        }

        public override void Execute()
        {
            try
            {
                Console.WriteLine("Статистика находится в разработке");
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                Console.WriteLine("Ошибка во время показа статистики");
            }
            finally
            {
                Console.WriteLine("Press any key to continue...");
                Console.ReadLine();
            }
        }
    }
}