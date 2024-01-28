using OpenQA.Selenium;

using System.Text.RegularExpressions;

namespace Giveaway.SteamGifts.Pages.SteamGift.Elements
{
    internal class GiveawayElement : BaseElement
    {
        private By GameNameSelector => By.CssSelector("a.giveaway__heading__name");
        private By GiveawayUrlSelector => By.CssSelector("a.giveaway__heading__name");
        private By SteamUrlSelector => By.CssSelector("a.giveaway__icon");
        private By PointsSelector => By.CssSelector("span.giveaway__heading__thin");
        private By LevelSelector => By.CssSelector("div.giveaway__columns div.giveaway__column--contributor-level");

        public GiveawayElement(IWebDriver webDriver, IWebElement webElement) : base(webDriver, webElement)
        {
        }

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

        public bool IsCollection()
        {
            var giveawayUrl = GetSteamUrl();
            bool isCollection = giveawayUrl.Contains("sub");
            return isCollection;
        }

        public int GetPoints()
        {
            var pointElement = WebElement.FindElements(PointsSelector).First(e => e.Text.EndsWith("P)")).Text;
            var pointText = pointElement.Trim('(', ')').TrimEnd('P');
            var point = int.Parse(pointText);
            return point;
        }

        public int GetApplicationId()
        {
            var steamUrl = GetSteamUrl();
            string[] patterns = [
                @"store.steampowered.com\/app\/(\d+)\/",
                @"store.steampowered.com\/sub\/(\d+)\/"
            ];
            foreach (var pattern in patterns)
            {
                if (Regex.IsMatch(steamUrl, pattern))
                {
                    var applicationId = Regex.Match(steamUrl, pattern).Groups[1].Value;
                    return Convert.ToInt32(applicationId);
                }
            }
            throw new FormatException("Error parsing ApplicationId. Invalid string format.");
        }

        public int GetLevel()
        {
            var levelElement = WebElement.FindElements(LevelSelector).FirstOrDefault();
            if (levelElement != null)
            {
                var levelText = levelElement.Text;
                Match match = Regex.Match(levelText, @"\d+");
                if (match.Success)
                {
                    return int.Parse(match.Value);
                }
                else
                {
                    throw new FormatException("Error parsing Level. Invalid string format.");
                }
            }
            else
            {
                return 0;
            }
        }
    }
}