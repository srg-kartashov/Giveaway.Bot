using Giveaway.SteamGifts.Pages.Giveaways;

using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Web;

namespace Giveaway.SteamGifts.Pages
{
    internal class WebNavigator : IDisposable
    {

        public IWebDriver Driver { get; set; }
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
                options.AddArgument("user-agent=Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/107.0.0.0 Safari/537.36");

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


        public GiveawayListPage GetGiveawayListPage(int pageNumber = 0)
        {
            var url = pageNumber == 0 ? BaseUrl : $"{BaseUrl}giveaways/search?page={pageNumber}";
            Driver.Navigate().GoToUrl(url);
            GiveawayListPage giveawaysPage = new GiveawayListPage(Driver);
            return giveawaysPage;
        }

        public GiveawayListPage GetNextGiveawayListPage()
        {
            var Uri = new Uri(Driver.Url);
            var queryPage = HttpUtility.ParseQueryString(Uri.Query).Get("page");
            if (queryPage != null)
            {
                var pageNumber = int.Parse(queryPage);
                return GetGiveawayListPage(pageNumber++);
            }
            return GetGiveawayListPage();
        }

        public GiveawayPage GetGiveawayPage(string giveawayUrl)
        {
            return new GiveawayPage(Driver, giveawayUrl);
        }

        public void Dispose()
        {
            Driver.Quit();
        }
    }
}
