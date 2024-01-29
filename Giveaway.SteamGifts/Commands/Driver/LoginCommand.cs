using Giveaway.SteamGifts.Models;
using Giveaway.SteamGifts.Pages;
using Giveaway.SteamGifts.Pages.SteamGift;

using NLog;

using OpenQA.Selenium;

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
            IWebDriver webDriver = null!;
            try
            {
                webDriver = new SeleniumDriverBuilder()
                  .SetUserDataPath(Configuration.DriverProfilePath)
                  .SetHeadless(false)
                  .Build();
                var steamGiftPage = new SteamGiftPage(webDriver);
                steamGiftPage.GoToPage(1);
                while (!steamGiftPage.IsAuthorized())
                {
                    Console.WriteLine("Вы не авторизиваны. Войдите пожалуйста в аккаунт");
                    Console.WriteLine("Повторная проверка через 10 секунд");
                    Thread.Sleep(10000);
                }
                Console.WriteLine("Авторизация прошла успешно");
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                Console.WriteLine("Ошибка во время авторизации");
            }
            finally
            {
                webDriver.Dispose();
                Console.WriteLine("Press any key to continue...");
                Console.ReadLine();
            }
        }
    }
}