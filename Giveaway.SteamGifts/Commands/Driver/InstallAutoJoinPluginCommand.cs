using Giveaway.SteamGifts.Models;
using Giveaway.SteamGifts.Pages;

namespace Giveaway.SteamGifts.Commands
{
    internal class InstallAutoJoinPluginCommand : BaseCommand
    {

        public InstallAutoJoinPluginCommand(Configuration configuration) : base(configuration)
        {
         
        }

        public override void Execute()
        {
            using (var webNavigator = new WebNavigator())
            {
                webNavigator.Start();
                webNavigator.GoToAutoJoinPluginPage();
                Console.WriteLine("Жми установить в браузере. Как будет готово - жми любую клавишу");
                Console.ReadLine();
                webNavigator.Dispose();
            }
        }
    }
}
