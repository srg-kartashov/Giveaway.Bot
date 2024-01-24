using Giveaway.SteamGifts.Models;

namespace Giveaway.SteamGifts.Commands
{
    internal abstract class BaseCommand
    {
        public Configuration Configuration { get; }

        public BaseCommand(Configuration configuration)
        {
            Configuration = configuration;
        }

        public abstract void Execute();
    }
}
