using OpenQA.Selenium.Chrome;
using OpenQA.Selenium;

namespace Giveaway.SteamGifts.Pages
{
    internal class SeleniumDriverBuilder
    {
        private string? DriverPath { get; set; }
        private bool Headless { get; set; }
        private string? UserDataPath { get; set; }

        public IWebDriver Build()
        {
            try
            {
                ChromeOptions options = new ChromeOptions();
                options.AddArgument("user-agent=Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/107.0.0.0 Safari/537.36");

                if (UserDataPath != null)
                {
                    string userProfilePath = Path.Combine(Directory.GetCurrentDirectory(), UserDataPath);
                    Directory.CreateDirectory(userProfilePath);
                    options.AddArguments("user-data-dir=" + userProfilePath);
                }

                if (Headless)
                {
                    options.AddArgument("--headless");
                    options.AddArgument("--window-size=1920,1080");
                }

                if (string.IsNullOrEmpty(DriverPath))
                {
                    ChromeDriverService service = ChromeDriverService.CreateDefaultService();
                    service.HideCommandPromptWindow = true;

                    return new ChromeDriver(service, options);
                }
                else
                {
                    return new ChromeDriver(DriverPath, options);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Ошибка во время запуска ChromeDriver", ex);
            }
        }

        public SeleniumDriverBuilder SetDriverPath(string path)
        {
            DriverPath = path;
            return this;
        }

        public SeleniumDriverBuilder SetHeadless(bool headless)
        {
            Headless = headless;
            return this;
        }

        public SeleniumDriverBuilder SetUserDataPath(string path)
        {
            UserDataPath = path;
            return this;
        }
    }
}