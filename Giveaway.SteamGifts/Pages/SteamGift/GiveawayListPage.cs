using Giveaway.SteamGifts.Pages.SteamGift.Elements;

using OpenQA.Selenium;

using System.Web;

namespace Giveaway.SteamGifts.Pages.SteamGift
{
    internal class GiveawayListPage : BasePage
    {
        private By CurrentPage => By.CssSelector("div.pagination__navigation a.is-selected span");
        private By Giveaways => By.CssSelector("div:not([class]) div:not([class]) div.giveaway__row-inner-wrap");
        private By Level => By.CssSelector("a[href^='/account'] span[title]");
        private By Pagination => By.CssSelector("div.pagination__navigation span");
        private By Points => By.CssSelector("a[href^='/account'] span.nav__points");
        private By UserName => By.CssSelector("header a[href^='/user']");

        public GiveawayListPage(IWebDriver driver) : base(driver)
        {
        }

        public bool CanNavigateNextPage()
        {
            var pagination = Driver.FindElements(Pagination).LastOrDefault();
            var nextPageExists = pagination?.Text == "Next";
            return nextPageExists;
        }

        public int GetCurrentPage()
        {
            var number = Driver.FindElements(CurrentPage)?.FirstOrDefault()?.Text;
            if (number == null)
            {
                if (Driver.Url.Trim('/').EndsWith("steamgifts.com"))
                    return 1;
                var Uri = new Uri(Driver.Url);
                number = HttpUtility.ParseQueryString(Uri.Query).Get("page");
            }
            if (int.TryParse(number, out var result))
                return result;
            throw new Exception("Не смогли определить номер текущей страницы");
        }

        public IEnumerable<GiveawayElement> GetGiveaways()
        {
            var giveaways = Driver.FindElements(Giveaways);

            foreach (var giveaway in giveaways)
            {
                yield return new GiveawayElement(Driver, giveaway);
            }
        }

        public int GetLevel()
        {
            var levelElement = Driver.FindElements(Level).LastOrDefault();
            if (levelElement != null)
            {
                var levelText = levelElement.Text;
                var levelValue = levelText.Split(' ').LastOrDefault();
                if (levelValue != null && int.TryParse(levelValue, out var level))
                {
                    return level;
                }
            }
            return 0;
        }

        public int GetPoints()
        {
            var points = Driver.FindElements(Points).FirstOrDefault();
            try
            {
                var textPoints = points?.Text?.Split(" ")?.LastOrDefault();
                return Convert.ToInt32(textPoints);
            }
            catch
            {
                return 0;
            }
        }

        public string GetUserName()
        {
            var userName = Driver.FindElements(UserName).FirstOrDefault();
            return userName?.GetAttribute("href")?.Split("/")?.Last() ?? string.Empty;
        }

        public bool IsAuthorized()
        {
            var data = Driver.WindowHandles;
            var userName = Driver.FindElements(UserName).FirstOrDefault();
            return userName != null && Driver.Url.Contains("https://www.steamgifts.com/");
        }
    }
}