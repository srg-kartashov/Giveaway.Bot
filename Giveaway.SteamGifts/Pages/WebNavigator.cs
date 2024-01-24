using Giveaway.SteamGifts.Pages.Giveaways;

using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

using System;
using System.Runtime.InteropServices;

namespace Giveaway.SteamGifts.Pages
{
    internal class WebNavigator : IDisposable
    {

        private IWebDriver Driver { get; set; }
        private string BaseUrl { get; } = "https://www.steamgifts.com/";
        public bool Headless { get; }

        public WebNavigator(bool headless = false)
        {
            Headless = headless;
        }

        public void Start()
        {
            ChromeOptions options = new ChromeOptions();
            string userProfilePath = Path.Combine(Directory.GetCurrentDirectory(), "UserProfile");
            Directory.CreateDirectory(userProfilePath);
            options.AddArguments("user-data-dir=" + userProfilePath);
            if (Headless)
            {
                options.AddArgument("--headless");
                options.AddArgument("--window-size=1920,1080");
            }
            var driverManager = new WebDriverManager.DriverManager();
            var config = new WebDriverManager.DriverConfigs.Impl.ChromeConfig();
            driverManager.SetUpDriver(config);
          
            ChromeDriverService service = ChromeDriverService.CreateDefaultService();
            service.HideCommandPromptWindow = true;
            Driver = new ChromeDriver(service, options);
            
        }

        internal void GoToAutoJoinPluginPage()
        {
            Driver.Navigate().GoToUrl("https://chromewebstore.google.com/detail/autojoin-for-steamgifts/bchhlccjhoedhhegglilngpbnldfcidc?hl=ru");
        }

        internal GiveawaysPage GoToGiveawaysPage(int pageNumber = 0)
        {
            var url = pageNumber == 0 ? BaseUrl : BaseUrl + $"{BaseUrl}giveaways/search?page={pageNumber}";
            Driver.Navigate().GoToUrl(url);
            GiveawaysPage giveawaysPage = new GiveawaysPage(Driver);
            return giveawaysPage;
        }

        // TODO подумать как 
        internal bool GoToNextGiveawaysPage(GiveawaysPage giveawaysPage)
        {
            var currentPage = giveawaysPage.GetCurrentPage();
            if (giveawaysPage.CanNavigateNextPage())
            {
                giveawaysPage = GoToGiveawaysPage(currentPage + 1);
                return true;
            }
            return false;
        }

        public void Dispose()
        {
            Driver.Quit();
        }
    }
}
