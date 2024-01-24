using Giveaway.SteamGifts.Models;

namespace Giveaway.SteamGifts.Commands
{
    internal class DriverRemoveProfileCommand : BaseCommand
    {
        public string DriverProfilePath { get; set; }
        public DriverRemoveProfileCommand(Configuration configuration) : base(configuration)
        {
            DriverProfilePath = configuration.DriverProfilePath;
        }

        public override void Execute()
        {
            Directory.Delete(DriverProfilePath, true);
            Console.WriteLine("Профиль ChromeDriver успешно удален...");
            Console.ReadLine();
        }
    }
}
