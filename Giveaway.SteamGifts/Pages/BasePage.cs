using OpenQA.Selenium;

namespace Giveaway.SteamGifts.Pages
{
    internal class BasePage
    {
        protected IWebDriver Driver { get; }
        public BasePage(IWebDriver driver)
        {
            Driver = driver;
        }

    }
}
