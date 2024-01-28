using Giveaway.SteamGifts.Models;
using Giveaway.SteamGifts.Pages;

using NLog;

namespace Giveaway.SteamGifts.Commands
{
    internal class LoginCommand : BaseCommand
    {
        private Logger Logger { get; set; } = LogManager.GetCurrentClassLogger();
        public LoginCommand(Configuration configuration) : base(configuration)
        {
        }

        public override void Execute()
        {
            try
            {
                using (var webNavigator = new PageNavigator(Configuration.DriverProfilePath, false))
                {
                    var giveawayPage = webNavigator.GetGiveawayListPage();
                    while (!giveawayPage.IsAuthorized())
                    {
                        Console.WriteLine("Вы не авторизиваны. Войдите пожалуйста в аккаунт");
                        Console.WriteLine("Повторная проверка через 10 секунд");
                        Thread.Sleep(10000);
                    }
                    Console.WriteLine("Авторизация прошла успешно");
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                Console.WriteLine("Ошибка во время авторизации");
            }
            finally
            {
                Console.WriteLine("Press any key to continue...");
                Console.ReadLine();
            }
        }
    }
}
