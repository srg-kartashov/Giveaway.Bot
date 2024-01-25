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
        }


        public override void Execute()
        {

            try
            {
                using (var webNavigator = new WebNavigator(Configuration.DriverProfilePath, Headless))
                {
                    GiveawaysPage giveawayPage = webNavigator.GoToGiveawaysPage();
                    RandomWaiter.WaitSeconds(10, 20);
                    if (!giveawayPage.IsAuthorized())
                    {
                        CombinedLogger.LogWarning("Вы не авторизиваны. Авторизируйтесь вручную для корректной работы приложения");
                        return;
                    }
                    CombinedLogger.LogInfo("Имя пользователя: " + giveawayPage.GetUserName() + " Уровень: " + giveawayPage.GetLevel() + " Баланс: " + giveawayPage.GetPoints());


                    do
                    {
                        foreach (var give in giveawayPage.GetGiveaways())
                        {
                            if (give.NoPoints)
                            {
                                CombinedLogger.LogInfo("Заканчиваю работу. Очки закончились.");
                                return;
                            }
                            ProcessGiveaway(give);
                        }
                    }
                    while (webNavigator.GoToNextGiveawaysPage(giveawayPage));
                }
            }
            catch (Exception ex)
            {
                CombinedLogger.LogError($"Ошибка во время выполнения {nameof(StartCommand)}", ex);
            }
        }

        public void ProcessGiveaway(GiveawayElement giveaway)
        {
            if (giveaway.AlreadyEntered)
                return;

            var filtered = FilterGame(giveaway, out var gameInfo);
            Logger.Info(GetGameNLog(giveaway, gameInfo, filtered));
            if (filtered)
            {
                giveaway.Focus();
                RandomWaiter.WaitSeconds(5, 10);
                giveaway.Enter();
                RandomWaiter.WaitSeconds(10, 20);
                var successEntered = giveaway.AlreadyEntered;
                TelegramService.SendMessage(GetGameTelegramMessage(giveaway, gameInfo), false);
                RandomWaiter.WaitSeconds(10, 15);
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

        private string GetGameNLog(GiveawayElement giveawayElement, SteamGameInfo gameInfo, bool filterResult)
        {
            StringBuilder loggerMessage = new StringBuilder();
            loggerMessage.Append($"[{gameInfo.TotalReviews} - {gameInfo.Raiting}%]");
            loggerMessage.Append($"Action: ");
            loggerMessage.Append(filterResult ? "Join; " : "Skip; ");
            loggerMessage.Append($"Name: {giveawayElement.GameName}; ");
            if (giveawayElement.IsCollection) loggerMessage.Append(" IsCollection;");
            if (giveawayElement.AlreadyEntered) loggerMessage.Append(" AlreadyEntered;");
            if (giveawayElement.NoPoints) loggerMessage.Append(" NoPoints;");
            return loggerMessage.ToString();
        }

        private string GetGameTelegramMessage(GiveawayElement giveawayElement, SteamGameInfo gameInfo)
        {
            StringBuilder telegramMessage = new StringBuilder();
            telegramMessage.Append(giveawayElement.AlreadyEntered ? "✅" : "⚠️");
            telegramMessage.Append($"<a href =\"{giveawayElement.GameUrl}\">{giveawayElement.GameName}</a>");
            telegramMessage.Append($" [{gameInfo.TotalReviews} - {gameInfo.Raiting}%] ");
            telegramMessage.Append($" <a href =\"{giveawayElement.GiveawayUrl}\">🌐</a>");
            return telegramMessage.ToString();
        }
    }
}
