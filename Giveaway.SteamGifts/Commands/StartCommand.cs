using Giveaway.Steam;
using Giveaway.Steam.Models;
using Giveaway.SteamGifts.Formatter;
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
        private FilterSettings[] HideFilters { get; }
        private FilterSettings[] EnterFilters { get; }
        private SteamClient SteamClient { get; }
        public RandomWaiter RandomWaiter { get; set; }
        public CombinedLogger CombinedLogger { get; }
        public Statistic Statistic { get; set; }
        public LogFormatter LogFormatter { get; set; }
        public TelegramFormatter TelegramFormatter { get; set; }
        public int Points { get; set; }

        ILogger Logger { get; } = LogManager.GetCurrentClassLogger();

        public bool Headless { get; set; }

        public StartCommand(Configuration configuration, bool headlessMode = false) : base(configuration)
        {
            TelegramService = new TelegramService(configuration.Telegram.BotToken, configuration.Telegram.ChatId);
            HideFilters = configuration.HideFilters;
            EnterFilters = configuration.EnterFilters;
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
                    Points = giveawayPage.GetPoints().GetValueOrDefault(0);

                    bool firstPage = true;

                    do
                    {
                        //int hiddenPerPage = Statistic.Hidden;
                        if (!firstPage)
                        {
                            giveawayPage = webNavigator.GetNextGiveawayListPage();
                        }
                        firstPage = false;
                        Logger.Info($"Начинаем обработку страницы {giveawayPage.GetCurrentPage()}");
                        RandomWaiter.WaitSeconds(5, 10);


                        foreach (var giveaway in giveawayPage.GetGiveaways().OrderByDescending(e => e.Level))
                        {
                            if (giveaway.AlreadyEntered) continue;

                            if (Points < giveaway.Points && Configuration.StopAfterPointsEnded)
                            {
                                CombinedLogger.LogInfo("Заканчиваю работу. Очки закончились.");
                                return;
                            }
                            try
                            {
                                ProcessGiveaway(giveaway, webNavigator);
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
                    TelegramService.SendMessage(TelegramFormatter.FormatForLog($"Ошибка во время выполнения {nameof(StartCommand)}", ex));
                    Logger.Error(TelegramFormatter.FormatForLog($"Ошибка во время выполнения {nameof(StartCommand)}", ex));
                }
                catch { }
            }
            finally
            {
                Logger.Info(LogFormatter.FormatForLog(Statistic));
                TelegramService.SendMessage(TelegramFormatter.FormatForLog(Statistic));
            }
        }

        public void ProcessGiveaway(GiveawayElement giveaway, WebNavigator webNavigator)
        {
            bool enoughtPoints = Points > giveaway.Points;


            if (giveaway.IsCollection)
            {
                if (Configuration.EnterCollections)
                {
                    if (enoughtPoints)
                    {
                        Logger.Info(GetGameNLog(giveaway, new SteamGameInfo(), "Try Enter Collection; "));
                        EnterGiveawayProcess(giveaway, webNavigator, new SteamGameInfo());
                    }
                    else
                    {
                        Logger.Info(GetGameNLog(giveaway, new SteamGameInfo(), "No Points; "));
                    }
                }
                else
                {
                    Logger.Info(GetGameNLog(giveaway, new SteamGameInfo(), "Skip Collection; "));
                    Statistic.Skiped++;
                }
                return;
            }


            var gameData = SteamClient.GetGameInfo(giveaway.ApplicationId);

            if (gameData == null)
            {
                CombinedLogger.LogWarning($"Не получили SteamInfo к игре {giveaway.ApplicationId}");
                Statistic.Failed++;
                return;
            }

            if (FilterGame(giveaway, EnterFilters, gameData) && enoughtPoints)
            {
                Logger.Info(GetGameNLog(giveaway, gameData, "Try Enter; "));
                if (EnterGiveawayProcess(giveaway, webNavigator, gameData))
                    RandomWaiter.WaitSeconds(5, 15);
                return;
            }

            if (FilterGame(giveaway, HideFilters, gameData))
            {
                Logger.Info(GetGameNLog(giveaway, gameData, "Try Hide; "));
                if (OpenPageHideGiveaway(giveaway, webNavigator))
                {
                    TelegramService.SendMessage(GetGameTelegramMessage(giveaway, gameData, GameResult.Hidden), Configuration.Telegram.Preview);
                    Statistic.Hidden++;
                }
                else
                {
                    Logger.Warn(GetGameNLog(giveaway, gameData, "Failed Hide"));
                }
                RandomWaiter.WaitSeconds(5, 10);
                return;
            }

            if (giveaway.IsCollection && !Configuration.EnterCollections)
            {
                Logger.Info(GetGameNLog(giveaway, gameData, "Skip Collection; "));
                Statistic.Skiped++;
                return;
            }

            if (!enoughtPoints)
            {
                Logger.Info(GetGameNLog(giveaway, gameData, "No Points; "));
                Statistic.Skiped++;
                return;
            }




        }

        public bool EnterGiveawayProcess(GiveawayElement giveaway, WebNavigator webNavigator, SteamGameInfo gameInfo)
        {
            int tryCount = 2;
            do
            {
                var success = OpenPageEnterGiveaway(giveaway, webNavigator);

                if (success)
                {
                    Points -= giveaway.Points;
                    Statistic.Entered++;
                    Logger.Info(GetGameNLog(giveaway, gameInfo, "Success"));
                    TelegramService.SendMessage(GetGameTelegramMessage(giveaway, gameInfo, GameResult.Entered), Configuration.Telegram.Preview);
                    Logger.Info("Успешно вступили");
                    RandomWaiter.WaitSeconds(10, 15);
                    return true;
                }
                else if (!success && tryCount == 0)
                {
                    Statistic.Failed++;
                    Logger.Info(GetGameNLog(giveaway, gameInfo, "Failed"));
                    TelegramService.SendMessage(GetGameTelegramMessage(giveaway, gameInfo, GameResult.Failed), Configuration.Telegram.Preview);
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
            return false;
        }

        public bool OpenPageEnterGiveaway(GiveawayElement giveaway, WebNavigator webNavigator)
        {
            bool result = false;
            giveaway.Focus();
            var currentWindowName = webNavigator.Driver.WindowHandles.First();
            try
            {
                using (var giveawayPage = webNavigator.GetGiveawayPage(giveaway.GiveawayUrl))
                {
                    RandomWaiter.WaitSeconds(2, 5);
                    giveawayPage.Enter();
                    RandomWaiter.WaitSeconds(2, 3);
                    result = giveawayPage.IsEntered();
                }
            }
            finally
            {
                webNavigator.Driver.SwitchTo().Window(currentWindowName);
            }
            return result;
        }

        public bool OpenPageHideGiveaway(GiveawayElement giveaway, WebNavigator webNavigator)
        {
            bool result = false;
            giveaway.Focus();
            var currentWindowName = webNavigator.Driver.WindowHandles.First();
            try
            {
                using (var giveawayPage = webNavigator.GetGiveawayPage(giveaway.GiveawayUrl))
                {
                    RandomWaiter.WaitSeconds(3, 5);
                    if (giveawayPage.IsHidden())
                        return true;
                    giveawayPage.ClickHideButton();
                    RandomWaiter.WaitSeconds(2, 3);
                    giveawayPage.ClickConfirmButton();
                    RandomWaiter.WaitSeconds(2, 3);
                    result = giveawayPage.IsHidden();
                }
            }
            finally
            {
                webNavigator.Driver.SwitchTo().Window(currentWindowName);
            }
            return result;
        }
        private bool FilterGame(GiveawayElement giveawayElement, FilterSettings[] filterSettings, SteamGameInfo gameData)
        {
            foreach (var filterSetting in filterSettings)
            {
                if (gameData.Raiting >= filterSetting.RatingFrom &&
                    gameData.Raiting <= filterSetting.RatingTo &&
                    gameData.TotalReviews >= filterSetting.ReviewsFrom &&
                    gameData.TotalReviews <= filterSetting.ReviewsTo)
                    return true;
            }
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

        enum GameResult { Entered, Failed, Hidden }
        private string GetGameTelegramMessage(GiveawayElement giveawayElement, SteamGameInfo gameInfo, GameResult gameResult)
        {

            StringBuilder telegramMessage = new StringBuilder();
            switch (gameResult)
            {
                case GameResult.Failed:
                    telegramMessage.Append("⚠️");
                    break;
                case GameResult.Entered:
                    telegramMessage.Append("✅");
                    break;
                case GameResult.Hidden:
                    telegramMessage.Append("👁");
                    break;
            }

            telegramMessage.Append($" <a href =\"{giveawayElement.GameUrl}\">{giveawayElement.GameName}</a>");
            telegramMessage.Append($" [{gameInfo.TotalReviews} - {gameInfo.Raiting}%] ");
            telegramMessage.Append($" <a href =\"{giveawayElement.GiveawayUrl}\">🌐</a>");
            return telegramMessage.ToString();
        }
    }
}
