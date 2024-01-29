using Giveaway.SteamGifts.Services;

using OpenQA.Selenium;

namespace Giveaway.SteamGifts.Pages
{
    internal class BasePage
    {
        protected IWebDriver Driver { get; }
        protected RandomWaiter RandomWaiter { get; }

        public BasePage(IWebDriver driver)
        {
            Driver = driver;
            RandomWaiter = new RandomWaiter();
        }

        public void RefrashPage()
        {
            Driver.Navigate().Refresh();
        }
    }
}