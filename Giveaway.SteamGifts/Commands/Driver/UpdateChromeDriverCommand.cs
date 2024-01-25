using Giveaway.SteamGifts.Models;

using NLog;

namespace Giveaway.SteamGifts.Commands
{
    internal class UpdateChromeDriverCommand : BaseCommand
    {
        public ILogger Logger => LogManager.GetCurrentClassLogger();
        public UpdateChromeDriverCommand(Configuration configuration) : base(configuration)
        {
        }

        public override void Execute()
        {
            try
            {
                Console.WriteLine("Пробуем обновить ChromeDriver");
                var driverManager = new WebDriverManager.DriverManager();
                var config = new WebDriverManager.DriverConfigs.Impl.ChromeConfig();
                driverManager.SetUpDriver(config);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                Console.WriteLine("Ошибка во время обновления ChromeDriver");
            }
            finally
            {
                Console.WriteLine("Press any key to continue...");
                Console.ReadLine();
            }
        }
    }
}
