using Giveaway.SteamGifts.Pages.SteamGift.Elements;

using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

using System.Web;

namespace Giveaway.SteamGifts.Pages.SteamGift
{
    internal class SteamGiftPage : BasePage
    {
        private readonly string baseUrl = "https://www.steamgifts.com/";
        private By CurrentPage => By.CssSelector("div.pagination__navigation a.is-selected span");
        private By Giveaways => By.CssSelector("div:not([class]) div:not([class]) div.giveaway__row-inner-wrap");
        private By Level => By.CssSelector("a[href^='/account'] span[title]");
        private By Pagination => By.CssSelector("div.pagination__navigation span");
        private By Points => By.CssSelector("a[href^='/account'] span.nav__points");
        private By UserName => By.CssSelector("header a[href^='/user']");

        public SteamGiftPage(IWebDriver driver) : base(driver)
        {
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

        public void GoToNextPage()
        {
            var Uri = new Uri(Driver.Url);
            var queryPage = HttpUtility.ParseQueryString(Uri.Query).Get("page");
            if (queryPage != null)
            {
                var pageNumber = int.Parse(queryPage);
                GoToPage(pageNumber + 1);
            }
            else
            {
                GoToPage(2);
            }
        }

        public void GoToPage(int pageNumber)
        {
            WebDriverWait wait = new WebDriverWait(Driver, TimeSpan.FromSeconds(10));
            string url = pageNumber == 1 ? baseUrl : $"{baseUrl}giveaways/search?page={pageNumber}";
            Driver.Navigate().GoToUrl(url);
            wait.Until(e => IsUserNameVisible());
        }

        public bool IsAuthorized()
        {
            var userName = Driver.FindElements(UserName).FirstOrDefault();
            return userName != null && Driver.Url.Contains("https://www.steamgifts.com/");
        }

        public bool IsNextPageAvailable()
        {
            var pagination = Driver.FindElements(Pagination).LastOrDefault();
            var nextPageExists = pagination?.Text == "Next";
            return nextPageExists;
        }

        public bool PerformOpenAndHideGiveawayPage(GiveawayElement giveaway)
        {
            bool result = false;
            giveaway.Focus();
            var currentWindowHandle = Driver.WindowHandles.First();
            try
            {
                RandomWaiter.WaitSeconds(1, 2);
                using (var giveawayPage = new GiveawayPage(Driver, giveaway.GetGiveawayUrl()))
                {
                    result = giveawayPage.PerformHide();
                }
            }
            finally
            {
                Driver.SwitchTo().Window(currentWindowHandle);
            }
            RandomWaiter.WaitSeconds(1, 2);
            return result;
        }

        public bool PerformOpenAndJoinGiveawayPage(GiveawayElement giveaway)
        {
            bool result = false;
            giveaway.Focus();
            var currentWindowHandle = Driver.WindowHandles.First();
            try
            {
                RandomWaiter.WaitSeconds(1, 2);
                using (var giveawayPage = new GiveawayPage(Driver, giveaway.GetGiveawayUrl()))
                {
                    result = giveawayPage.PerformEnter();
                }
                RandomWaiter.WaitSeconds(1, 2);
            }
            finally
            {
                Driver.SwitchTo().Window(currentWindowHandle);
            }
            return result;
        }

        private bool IsUserNameVisible()
        {
            var userName = Driver.FindElements(UserName).FirstOrDefault();
            return userName != null;
        }
    }
}