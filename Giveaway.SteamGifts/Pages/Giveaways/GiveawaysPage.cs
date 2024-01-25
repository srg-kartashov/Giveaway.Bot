using Giveaway.SteamGifts.Pages.Giveaways.Elements;

using OpenQA.Selenium;

namespace Giveaway.SteamGifts.Pages.Giveaways
{
    internal class GiveawaysPage : BasePage
    {
        private By SignInButton => By.CssSelector("header a[href$='login']");
        private By Pagination => By.CssSelector("div.pagination__navigation span");
        private By UserName => By.CssSelector("header a[href^='/user']");
        private By Points => By.CssSelector("a[href^='/account'] span.nav__points");
        private By Level => By.CssSelector("a[href^='/account'] span[title]");
        private By Giveaways => By.CssSelector("div#posts div.giveaway__row-outer-wrap");
        private By CurrentPage => By.CssSelector("div.pagination__navigation a.is-selected");

        public GiveawaysPage(IWebDriver driver) : base(driver)
        {
        }

        public bool IsAuthorized()
        {
            var userName = Driver.FindElements(UserName).FirstOrDefault();
            return userName != null && Driver.Url.Contains("https://www.steamgifts.com/");
        }

        public bool CanNavigateNextPage()
        {
            var pagination = Driver.FindElements(Pagination).LastOrDefault();
            var nextPageExists = pagination?.Text == "Next";
            return nextPageExists;
        }

        public string GetUserName()
        {
            var userName = Driver.FindElements(UserName).FirstOrDefault();
            return userName?.GetAttribute("href")?.Split("/")?.Last() ?? string.Empty;
        }

        public int? GetPoints()
        {
            var points = Driver.FindElements(Points).FirstOrDefault();
            try
            {
                var textPoints = points?.Text?.Split(" ")?.LastOrDefault();
                return Convert.ToInt32(textPoints);
            }
            catch
            {
                return null;
            }
        }

        public string GetLevel()
        {
            var level = Driver.FindElements(Level).LastOrDefault();
            return level?.Text ?? string.Empty;
        }

        public int GetCurrentPage()
        {
            var number = Driver.FindElement(CurrentPage).GetAttribute("data-page-number");
            return Convert.ToInt32(number);
        }

        public IEnumerable<GiveawayElement> GetGiveaways()
        {
            var giveaways = Driver.FindElements(Giveaways);
            foreach (var giveaway in giveaways)
            {
                yield return new GiveawayElement(Driver, giveaway);
            }
        }

    }
}
