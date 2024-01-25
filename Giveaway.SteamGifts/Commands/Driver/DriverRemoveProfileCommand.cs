using Giveaway.SteamGifts.Models;

using NLog;

namespace Giveaway.SteamGifts.Commands
{
    internal class DriverRemoveProfileCommand : BaseCommand
    {
        public ILogger Logger => LogManager.GetCurrentClassLogger();
        public string DriverProfilePath { get; set; }
        public DriverRemoveProfileCommand(Configuration configuration) : base(configuration)
        {
            DriverProfilePath = configuration.DriverProfilePath;
        }

        public override void Execute()
        {

            try
            {
                if (Directory.Exists(DriverProfilePath))
                {
                    Directory.Delete(DriverProfilePath, true);
                    Console.WriteLine("Профиль ChromeDriver успешно удален...");
                }
                else
                {
                    Console.WriteLine($"Профиль по пути {Path.Combine(Directory.GetCurrentDirectory(), DriverProfilePath)} отсутствует");
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                Console.WriteLine("Ошибка во время удаления профиля");
            }
            finally
            {
                Console.WriteLine("Press any key to continue...");
                Console.ReadLine();
            }
        }
    }
}
