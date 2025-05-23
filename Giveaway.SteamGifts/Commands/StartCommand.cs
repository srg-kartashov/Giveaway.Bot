﻿using Giveaway.Steam;
using Giveaway.Steam.Models;
using Giveaway.SteamGifts.Filters;
using Giveaway.SteamGifts.Formatter;
using Giveaway.SteamGifts.Models;
using Giveaway.SteamGifts.Pages;
using Giveaway.SteamGifts.Pages.SteamGift;
using Giveaway.SteamGifts.Pages.SteamGift.Elements;
using Giveaway.SteamGifts.Services;

using NLog;

using OpenQA.Selenium;

namespace Giveaway.SteamGifts.Commands
{
    internal class StartCommand : BaseCommand
    {
        public CombinedLogger CombinedLogger { get; }
        public bool Headless { get; set; }
        public bool IsAuto { get; }
        public LogFormatter LogFormatter { get; set; }
        public int Points { get; set; }
        public RandomWaiter RandomWaiter { get; set; }
        public Statistic Statistic { get; set; }
        public TelegramFormatter TelegramFormatter { get; set; }
        private FilterSettings[] EnterFilters { get; }
        private FilterSettings[] HideFilters { get; }
        private ILogger Logger { get; } = LogManager.GetCurrentClassLogger();
        private SteamClient SteamClient { get; }
        private TelegramService TelegramService { get; }
        private TelegramSettings TelegramSettings { get; }

        public StartCommand(Configuration configuration, bool headlessMode = false, bool isAuto = false) : base(configuration)
        {
            TelegramService = new TelegramService(configuration.Telegram.BotToken, configuration.Telegram.ChatId);
            HideFilters = configuration.HideFilters;
            EnterFilters = configuration.EnterFilters;
            TelegramSettings = configuration.Telegram;
            SteamClient = new SteamClient();
            RandomWaiter = new RandomWaiter();
            CombinedLogger = new CombinedLogger(Logger, TelegramService);
            Headless = headlessMode;
            IsAuto = isAuto;
            Statistic = new Statistic();
            LogFormatter = new LogFormatter();
            TelegramFormatter = new TelegramFormatter();
        }

        public override void Execute()
        {
            IWebDriver webDriver = null!;
            try
            {
                if (Configuration.StartingDelayInMinutesFrom != 0 && Configuration.StartingDelayInMinutesTo != 0)
                {
                    var waitBeforeStart = new Random().Next(Configuration.StartingDelayInMinutesFrom, Configuration.StartingDelayInMinutesTo);
                    CombinedLogger.LogInfo("Ожидаем перед запуском " + waitBeforeStart);
                    Thread.Sleep(waitBeforeStart * 1000 * 60);
                }

                webDriver = new SeleniumDriverBuilder()
                   .SetUserDataPath(Configuration.DriverProfilePath)
                   .SetHeadless(Headless)
                   .SetDriverPath(Configuration.ChromeDriverDirectory)
                   .Build();

                CombinedLogger.LogInfo("Начинаю работу");
                SteamGiftPage steamGiftPage = new SteamGiftPage(webDriver);
                steamGiftPage.GoToPage(1);
                RandomWaiter.WaitSeconds(10, 20);

                Logger.Trace("Проверяем авторизацию");
                if (!steamGiftPage.IsAuthorized())
                {
                    CombinedLogger.LogWarning("Вы не авторизиваны. Авторизируйтесь вручную для корректной работы приложения");
                    return;
                }

                var userData = new UserData(steamGiftPage.GetUserName(), steamGiftPage.GetLevel(), steamGiftPage.GetPoints());
                Logger.Info(LogFormatter.FormatForLog(userData));
                TelegramService.SendMessage(TelegramFormatter.FormatForLog(userData));
                Points = userData.Points;

                HashSet<string> processedGiveaways = new HashSet<string>();
                do
                {
                    Logger.Trace($"Начинаем обработку страницы [{steamGiftPage.GetCurrentPage()}]");
                    RandomWaiter.WaitSeconds(5, 10);

                    int hiddenCount = Statistic.Hidden;

                    var giveawaysList = steamGiftPage.GetGiveaways()
                        .Where(e => !processedGiveaways.Contains(e.GetGiveawayUrl()))
                        .OrderByDescending(e => e.GetLevel());

                    int count = giveawaysList.Count();
                    int index = 1;
                    foreach (var giveaway in giveawaysList)
                    {
                        Console.WriteLine($"Game number [{index++}/{count}]");
                        if (giveaway.HasAlreadyJoined()) continue;

                        if (Points < giveaway.GetPoints() && Configuration.StopAfterPointsEnded)
                        {
                            CombinedLogger.LogInfo("Заканчиваю работу. Очки закончились.");
                            return;
                        }
                        try
                        {
                            ProcessGiveaway(steamGiftPage, giveaway);
                            processedGiveaways.Add(giveaway.GetGiveawayUrl());
                        }
                        catch (Exception ex)
                        {
                            Statistic.Failed++;
                            CombinedLogger.LogError("Ошибка во время обработки игры", ex);
                        }
                    }
                    if (hiddenCount < Statistic.Hidden)
                    {
                        Console.WriteLine("Refrash per page " + (Statistic.Hidden - hiddenCount));
                        Console.WriteLine("Again");
                        hiddenCount = Statistic.Hidden;
                        steamGiftPage.RefrashPage();
                        continue;
                    }
                    else
                    {
                        if (steamGiftPage.IsNextPageAvailable())
                        {
                            steamGiftPage.GoToNextPage();
                            continue;
                        }
                        else
                            return;
                    }
                }
                while (true);
            }
            catch (Exception ex)
            {
                CombinedLogger.LogError($"Ошибка во время выполнения {nameof(StartCommand)}", ex);
            }
            finally
            {
                webDriver.Quit();
                Logger.Info(LogFormatter.FormatForLog(Statistic));
                TelegramService.SendMessage(TelegramFormatter.FormatForLog(Statistic));
                if (!Headless && !IsAuto)
                {
                    Console.WriteLine("Press any key to continue...");
                    Console.ReadLine();
                }
            }
        }

        public void ProcessGiveaway(SteamGiftPage steamGiftPage, GiveawayElement giveawayElement)
        {
            bool enoughtPoints = Points > giveawayElement.GetPoints();
            if (!enoughtPoints && Configuration.StopAfterPointsEnded)
            {
                return;
            }

            var gameStatistic = giveawayElement.IsCollection() ? new SteamGameInfo() : SteamClient.GetGameInfo(giveawayElement.GetApplicationId());
            var giveawayData = new GiveawayData(giveawayElement, gameStatistic);

            BaseFilterHandler[] collectionJoinFilters =
                [
                    new CollectionFilterHandler(Configuration.EnterCollections),
                    new PriceFilterHandler(Points)
                ];
            BaseFilterHandler[] gameJoinFilters =
              [
                  new NonCollectionFilterHandler(),
                  new PriceFilterHandler(Points),
                  new RatingReviewFilterHandler(EnterFilters),
              ];
            if (collectionJoinFilters.All(e => e.Filter(giveawayData)) || gameJoinFilters.All(e => e.Filter(giveawayData)))
            {
                Logger.Trace(LogFormatter.FormatForLog(giveawayData, GiveawayAction.TryJoin));
                if (steamGiftPage.PerformOpenAndJoinGiveawayPage(giveawayElement))
                {
                    Statistic.Joined++;
                    Points -= giveawayData.Points;
                    Logger.Info(LogFormatter.FormatForLog(giveawayData, GiveawayAction.Join));
                    TelegramService.SendMessage(TelegramFormatter.FormatForLog(giveawayData, GiveawayAction.Join), TelegramSettings.PreviewJoinedGiveaways);
                }
                else
                {
                    Statistic.Failed++;
                    Logger.Warn(LogFormatter.FormatForLog(giveawayData, GiveawayAction.Failed));
                    TelegramService.SendMessage(TelegramFormatter.FormatForLog(giveawayData, GiveawayAction.Failed));
                }
                RandomWaiter.WaitSeconds(3, 5);
                return;
            }

            BaseFilterHandler[] hideFilters =
              [
                  new RatingReviewFilterHandler(HideFilters),
                  new NonCollectionFilterHandler()
              ];
            if (hideFilters.All(e => e.Filter(giveawayData)))
            {
                Logger.Trace(LogFormatter.FormatForLog(giveawayData, GiveawayAction.TryHide));
                if (steamGiftPage.PerformOpenAndHideGiveawayPage(giveawayElement))
                {
                    Statistic.Hidden++;
                    Logger.Info(LogFormatter.FormatForLog(giveawayData, GiveawayAction.Hide));
                    TelegramService.SendMessage(TelegramFormatter.FormatForLog(giveawayData, GiveawayAction.Hide), TelegramSettings.PreviewHiddenGiveaways);
                }
                else
                {
                    Statistic.FailedHidden++;
                    Logger.Warn(LogFormatter.FormatForLog(giveawayData, GiveawayAction.FailedHide));
                    TelegramService.SendMessage(TelegramFormatter.FormatForLog(giveawayData, GiveawayAction.FailedHide));
                }
                RandomWaiter.WaitSeconds(3, 5);
                return;
            }

            Logger.Trace(LogFormatter.FormatForLog(giveawayData, GiveawayAction.Skip));
            Statistic.Skiped++;
        }
    }
}