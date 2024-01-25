using Giveaway.Steam;
using Giveaway.Steam.Models;
using Giveaway.SteamGifts.Models;
using Giveaway.SteamGifts.Pages;
using Giveaway.SteamGifts.Pages.Giveaways;
using Giveaway.SteamGifts.Pages.Giveaways.Elements;
using Giveaway.SteamGifts.Services;

using NLog;

using System.Text;

namespace Giveaway.SteamGifts.Commands
{

    internal class StartCommand : BaseCommand
    {
        private TelegramService TelegramService { get; }
        private TelegramSettings TelegramSettings { get; }
        private FilterSettings FilterSettings { get; }
        private SteamClient SteamClient { get; }
        public RandomWaiter RandomWaiter { get; set; }
        public CombinedLogger CombinedLogger { get; }
        public Statistic Statistic { get; set; }

        ILogger Logger { get; } = LogManager.GetCurrentClassLogger();

        public bool Headless { get; set; }

        public StartCommand(Configuration configuration, bool headlessMode = false) : base(configuration)
        {
            TelegramService = new TelegramService(configuration.Telegram.BotToken, configuration.Telegram.ChatId);
            FilterSettings = configuration.Filter;
            TelegramSettings = configuration.Telegram;
            SteamClient = new SteamClient();
            RandomWaiter = new RandomWaiter();
            CombinedLogger = new CombinedLogger(Logger, TelegramService);
            Headless = headlessMode;
            Statistic = new Statistic();
        }


        public override void Execute()
        {

            try
            {

                using (var webNavigator = new WebNavigator(Configuration.DriverProfilePath, Headless))
                {
                    CombinedLogger.LogInfo("Начинаю работу");
                    GiveawayListPage giveawayPage = webNavigator.GetGiveawayListPage();
                    RandomWaiter.WaitSeconds(10, 20);


                    Logger.Trace("Проверяем авторизацию");
                    if (!giveawayPage.IsAuthorized())
                    {
                        CombinedLogger.LogWarning("Вы не авторизиваны. Авторизируйтесь вручную для корректной работы приложения");
                        return;
                    }
                    Logger.Info("Имя пользователя: " + giveawayPage.GetUserName() + " Уровень: " + giveawayPage.GetLevel() + " Баланс: " + giveawayPage.GetPoints());
                    TelegramService.SendMessage($"👨‍💻 {giveawayPage.GetUserName()}\n📈 Уровень: {giveawayPage.GetLevel()}\n💰 Баланс: {giveawayPage.GetPoints()}");

                    bool firstPage = true;
                    do
                    {
                        if (!firstPage)
                        {
                            giveawayPage = webNavigator.GetNextGiveawayListPage();
                        }
                        Logger.Info($"Начинаем обработку страницы {giveawayPage.GetCurrentPage()}");
                        RandomWaiter.WaitSeconds(5, 10);

                       
                        foreach (var give in giveawayPage.GetGiveaways().OrderByDescending(e => e.Level))
                        {
                            if (give.AlreadyEntered) continue;

                            if (giveawayPage.GetPoints() < give.Points)
                            {
                                CombinedLogger.LogInfo("Заканчиваю работу. Очки закончились.");
                                return;
                            }
                            try
                            {
                                ProcessGiveaway(give, webNavigator);
                            }
                            catch (Exception ex)
                            {
                                Statistic.Failed++;
                                CombinedLogger.LogError("Ошибка во время обработки игры", ex);
                            }
                        }

                    }
                    while (giveawayPage.CanNavigateNextPage());
                }
            }
            catch (Exception ex)
            {
                try
                {

                    TelegramService.SendMessage($"Ошибка во время выполнения {nameof(StartCommand)}\n ```{ex.StackTrace?.Trim()}```");
                }
                catch { }
                throw;
            }
            finally
            {
                try
                {
                    CombinedLogger.LogInfo(Statistic.ToString());
                }
                catch { }
            }
        }

        public void ProcessGiveaway(GiveawayElement giveaway, WebNavigator webNavigator)
        {
            var filtered = FilterGame(giveaway, out var gameInfo);
            Logger.Info(GetGameNLog(giveaway, gameInfo, filtered ? "Try Enter;" : "Skip;"));
            if (filtered)
            {
                int tryCount = 2;
                do
                {
                    bool success = false;
                    giveaway.Focus();
                    var currentWindowName = webNavigator.Driver.WindowHandles.First();
                    try
                    {
                        using (var giveawayPage = webNavigator.GetGiveawayPage(giveaway.GiveawayUrl))
                        {
                            RandomWaiter.WaitSeconds(2, 5);
                            giveawayPage.Enter();
                            RandomWaiter.WaitSeconds(2, 5);
                            success = giveawayPage.IsEntered();
                        }
                    }
                    finally
                    {
                        webNavigator.Driver.SwitchTo().Window(currentWindowName);
                    }

                    if (success)
                    {
                        Statistic.Entered++;
                        Logger.Info(GetGameNLog(giveaway, gameInfo, "Success"));
                        TelegramService.SendMessage(GetGameTelegramMessage(giveaway, gameInfo, success), Configuration.Telegram.TelegramSendWithPreview);
                        Logger.Info("Успешно вступили");
                        RandomWaiter.WaitSeconds(10, 15);
                        break;
                    }
                    else if (!success && tryCount == 0)
                    {
                        Statistic.Failed++;
                        Logger.Info(GetGameNLog(giveaway, gameInfo, "Failed"));
                        TelegramService.SendMessage(GetGameTelegramMessage(giveaway, gameInfo, success), Configuration.Telegram.TelegramSendWithPreview);
                        RandomWaiter.WaitSeconds(5, 10);
                    }
                    else
                    {
                        Logger.Info(GetGameNLog(giveaway, gameInfo, "Try Again"));
                        Logger.Info("Не удалось вступить в раздачу");
                        Logger.Info($"Осталовь попыток {tryCount}");
                    }
                    tryCount--;

                }
                while (tryCount > 0);
            }
            else
            {
                Statistic.Skiped++;
            }
        }

        private bool FilterGame(GiveawayElement giveawayElement, out SteamGameInfo gameData)
        {
            gameData = new SteamGameInfo();
            if (giveawayElement.IsCollection && FilterSettings.EnterCollection)
                return true;

            gameData = SteamClient.GetGameInfo(giveawayElement.ApplicationId);

            if (gameData == null)
                return false;
            if (gameData.Raiting > FilterSettings.MinRatingForEnter && gameData.TotalReviews > FilterSettings.MinReviewsForEnter)
                return true;
            return false;
        }

        private string GetGameNLog(GiveawayElement giveawayElement, SteamGameInfo gameInfo, string action)
        {
            StringBuilder loggerMessage = new StringBuilder();
            loggerMessage.Append($"[{gameInfo.TotalReviews} - {gameInfo.Raiting}%]");
            loggerMessage.Append($"Name: {giveawayElement.GameName}; ");
            loggerMessage.Append($"Points: {giveawayElement.Points}; ");
            loggerMessage.Append($"Action: ");
            loggerMessage.Append(action);
            if (giveawayElement.IsCollection) loggerMessage.Append(" IsCollection;");
            return loggerMessage.ToString();
        }

        private string GetGameTelegramMessage(GiveawayElement giveawayElement, SteamGameInfo gameInfo, bool success)
        {

            StringBuilder telegramMessage = new StringBuilder();
            telegramMessage.Append(success ? "✅" : "⚠️");
            telegramMessage.Append($" <a href =\"{giveawayElement.GameUrl}\">{giveawayElement.GameName}</a>");
            telegramMessage.Append($" [{gameInfo.TotalReviews} - {gameInfo.Raiting}%] ");
            telegramMessage.Append($" <a href =\"{giveawayElement.GiveawayUrl}\">🌐</a>");
            return telegramMessage.ToString();
        }
    }
}
