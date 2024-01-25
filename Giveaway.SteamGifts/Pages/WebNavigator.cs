using Giveaway.SteamGifts.Pages.Giveaways;

using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace Giveaway.SteamGifts.Pages
{
    internal class WebNavigator : IDisposable
    {

        private IWebDriver Driver { get; set; }
        private string BaseUrl { get; } = "https://www.steamgifts.com/";
        public string UserProfilePath { get; }
        public bool Headless { get; }

        public WebNavigator(string userProfilePath1, bool headless)
        {
            UserProfilePath = userProfilePath1;
            Headless = headless;
            try
            {
                ChromeOptions options = new ChromeOptions();
                string userProfilePath = Path.Combine(Directory.GetCurrentDirectory(), UserProfilePath);
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
            catch (Exception ex)
            {
                throw new Exception("Ошибка во время запуска ChromeDriver", ex);
            }
        }

        public void GoToAutoJoinPluginPage()
        {
            Driver.Navigate().GoToUrl("https://chromewebstore.google.com/detail/autojoin-for-steamgifts/bchhlccjhoedhhegglilngpbnldfcidc?hl=ru");
        }

        public GiveawaysPage GoToGiveawaysPage(int pageNumber = 0)
        {
            var url = pageNumber == 0 ? BaseUrl : $"{BaseUrl}giveaways/search?page={pageNumber}";
            Driver.Navigate().GoToUrl(url);
            GiveawaysPage giveawaysPage = new GiveawaysPage(Driver);
            return giveawaysPage;
        }

        // TODO подумать как 
        public bool GoToNextGiveawaysPage(GiveawaysPage giveawaysPage)
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
            var cookies = Driver.Manage().Cookies.AllCookies;
            Driver.Quit();
        }
    }
}
