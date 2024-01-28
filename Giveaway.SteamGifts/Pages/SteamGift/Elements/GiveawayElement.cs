using Giveaway.SteamGifts.Extensions;

using OpenQA.Selenium;

using System;
using System.Text.RegularExpressions;

namespace Giveaway.SteamGifts.Pages.SteamGift.Elements
{
    internal class GiveawayElement : BaseElement
    {
        private By GameNameSelector => By.CssSelector("a.giveaway__heading__name");
        private By GiveawayUrlSelector => By.CssSelector("a.giveaway__heading__name");
        private By SteamUrlSelector => By.CssSelector("a.giveaway__icon");
        private By PointsSelector => By.CssSelector("span.giveaway__heading__thin");

        public string GetGameName()
        {
            var gameName = WebElement.FindElements(GameNameSelector).First();
            return gameName.Text;
        }
        public string GetSteamUrl()
        {
            var steamUrl = WebElement.FindElements(SteamUrlSelector).First().GetAttribute("href");
            return steamUrl;
        }
        public string GetGiveawayUrl()
        {
            var giveawayUrl = WebElement.FindElements(GiveawayUrlSelector).First().GetAttribute("href");
            return giveawayUrl;
        }
        public bool HasAlreadyJoined()
        {
            var hasAlreadyJoined = WebElement.GetAttribute("class").Contains("is-faded");
            return hasAlreadyJoined;
        }


        //public bool IsCollection()
        //{
        //    var giveawayUrl = GetGiveawayUrl();
        //    bool isCollection = giveawayUrl.Contains("sub");
        //    return isCollection;
        //}
        //public int GetPoints()
        //{
        //    var pointElement = WebElement.FindElements(PointsSelector).First();

        //}



        public string GameName => GetTextBySelector("a.giveaway__heading__name");//
        public string GameUrl => GetAttributeBySelector("a.giveaway__icon", "href");//
        public string GiveawayUrl => GetAttributeBySelector("a.giveaway__heading__name", "href");//
        public bool AlreadyEntered { get { return WebElement.GetAttribute("class").Contains("is-faded"); } }//
        public bool IsCollection => GameUrl.Contains("sub");
        public int ApplicationId => GetApplicationIdFromUrl(GameUrl);


        public int Points
        {
            get
            {
                var pointsElement = WebElement.FindElements(PointsSelector).FirstOrDefault(e => e.Text.Contains("P"))?.Text;
                var points = pointsElement?.TryParseInt32() ?? -1;
                if (points == -1)
                {
                    Console.WriteLine(GameName + "Проблемы с поинтами");
                }
                return points;
            }
        }
        public int Level => GetLevel();

        public GiveawayElement(IWebDriver webDriver, IWebElement webElement) : base(webDriver, webElement)
        {
        }

        private int GetLevel()
        {
            try
            {
                var level = GetTextBySelector("div.giveaway__columns div.giveaway__column--contributor-level");
                if (!string.IsNullOrEmpty(level))
                    return level.TryParseInt32().GetValueOrDefault(-1);
                else return 0;
            }
            catch
            {
                return 0;
            }
        }

        // TODO: Пересмотреть
        private int GetApplicationIdFromUrl(string url)
        {
            string gamePattern = @"store.steampowered.com\/app\/(\d+)\/";
            string collectionPattern = @"store.steampowered.com\/sub\/(\d+)\/";
            MatchCollection matches;
            if (Regex.IsMatch(url, gamePattern))
            {
                matches = Regex.Matches(url, gamePattern);
            }
            else if (Regex.IsMatch(url, collectionPattern))
            {
                matches = Regex.Matches(url, collectionPattern);
            }
            else
                throw new InvalidDataException();


            var result = matches.First().Groups.Values.Last().Value;
            return Convert.ToInt32(result);
        }




    }
}
