﻿using Giveaway.SteamGifts.Extensions;
using Giveaway.SteamGifts.Pages.Giveaways.Elements;

using OpenQA.Selenium;

using System.Web;

namespace Giveaway.SteamGifts.Pages.Giveaways
{
    internal class GiveawaysPage : BasePage
    {
        private By SignInButton => By.CssSelector("header a[href$='login']");
        private By Pagination => By.CssSelector("div.pagination__navigation span");
        private By UserName => By.CssSelector("header a[href^='/user']");
        private By Points => By.CssSelector("a[href^='/account'] span.nav__points");
        private By Level => By.CssSelector("a[href^='/account'] span[title]");
        private By Giveaways => By.CssSelector("div[data-game-id] div.giveaway__row-inner-wrap");
        private By CurrentPage => By.CssSelector("div.pagination__navigation a.is-selected span");

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
            var number = Driver.FindElements(CurrentPage)?.FirstOrDefault()?.Text;
            if (number == null)
            {
                if (Driver.Url.Trim('/').EndsWith("steamgifts.com"))
                    return 1;
                var Uri = new Uri(Driver.Url);
                number = HttpUtility.ParseQueryString(Uri.Query).Get("page");
            }
            return number?.ParseInteger() ?? throw new Exception("Не смогли определить номер текущей страницы");
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
