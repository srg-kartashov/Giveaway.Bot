using NLog;

namespace Giveaway.SteamGifts.Services
{
    internal class CombinedLogger
    {
        private ILogger Logger { get; }
        private TelegramService TelegramService { get; }

        public CombinedLogger(ILogger logger, TelegramService telegramService)
        {
            Logger = logger;
            TelegramService = telegramService;
        }

        public void LogInfo(string message)
        {
            Logger.Info(message);
            TelegramService.SendMessage("ℹ " + message);
        }

        public void LogError(string message, Exception ex)
        {
            Logger.Error(ex, message);
            TelegramService.SendMessage("🛑 " + message + "\n" + ex.Message + "\n" + "<pre>" + ex.StackTrace + "</pre>");
        }

        public void LogWarning(string message)
        {
            Logger.Warn(message);
            TelegramService.SendMessage("⚠ " + message);
        }
    }
}