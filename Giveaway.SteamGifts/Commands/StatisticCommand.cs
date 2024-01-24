using Giveaway.SteamGifts.Models;

namespace Giveaway.SteamGifts.Commands
{
    internal class StatisticCommand : BaseCommand
    {
        public StatisticCommand(Configuration configuration) : base(configuration)
        {
        }

        public override void Execute()
        {
            Console.WriteLine("Статистика находится в разработке");
            Console.WriteLine("Press any key to continue...");
            Console.ReadLine();
        }
    }
}
