using NLog;

namespace Giveaway.SteamGifts.Commands
{
    internal class CommandBehavior
    {
        public BaseCommand Command { get; }
        protected ILogger Logger { get; }

        public CommandBehavior(BaseCommand baseCommand)
        {
            Command = baseCommand;
            Logger = LogManager.GetCurrentClassLogger();
        }

        public void Execute()
        {
            try
            {
                Command.Execute();
            }
            catch (Exception e)
            {
                Logger.Error(e, $"Ошибка во время выполнения команды");
            }
        }

    }
}
