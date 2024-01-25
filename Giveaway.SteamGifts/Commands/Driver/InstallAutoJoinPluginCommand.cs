using Giveaway.SteamGifts.Models;
using Giveaway.SteamGifts.Pages;

using NLog;

namespace Giveaway.SteamGifts.Commands
{
    internal class InstallAutoJoinPluginCommand : BaseCommand
    {
        public ILogger Logger => LogManager.GetCurrentClassLogger();
        public InstallAutoJoinPluginCommand(Configuration configuration) : base(configuration)
        {

        }

        public override void Execute()
        {
            try
            {
                Console.WriteLine("Сейчас запустится ChromeDriver");
                using (var webNavigator = new WebNavigator(Configuration.DriverProfilePath, false))
                {
                    webNavigator.GoToAutoJoinPluginPage();
                    Console.WriteLine("Жми установить в браузере. Как будет готово - жми любую клавишу");
                    Console.ReadLine();
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                Console.WriteLine("Ошибка во время установки AutoJoin");
            }
        }
    }
}
