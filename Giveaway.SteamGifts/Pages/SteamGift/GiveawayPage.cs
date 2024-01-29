using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;

namespace Giveaway.SteamGifts.Pages.SteamGift
{
    internal class GiveawayPage : BasePage, IDisposable
    {
        public string Url { get; }

        private By ConfirmHideButtonSelector => By.CssSelector("div.form__submit-button");
        private By DeleteButtonSelector => By.CssSelector("form div[data-do='entry_delete'].sidebar__entry-delete");
        private By EnterButtonSelector => By.CssSelector("form div[data-do='entry_insert'].sidebar__entry-insert");
        private By HideButtonSelector => By.CssSelector("div.featured__heading i.featured__giveaway__hide");

        public GiveawayPage(IWebDriver driver, string url) : base(driver)
        {
            Url = url;
            Driver
                .SwitchTo()
                .NewWindow(WindowType.Tab)
                .Navigate()
                .GoToUrl(Url);
        }

        public void Dispose()
        {
            Driver.Close();
        }

        public bool PerformEnter()
        {
            WebDriverWait wait = new WebDriverWait(Driver, TimeSpan.FromSeconds(10));
            RandomWaiter.WaitSeconds(1, 3);
            ClickEnterButton();
            wait.Until(e => IsEntered());
            RandomWaiter.WaitSeconds(1, 3);
            return IsEntered();
        }

        public bool PerformHide()
        {
            WebDriverWait wait = new WebDriverWait(Driver, TimeSpan.FromSeconds(10));
            if (IsHidden())
                return true;
            ClickHideButton();
            wait.Until(e => IsConfirmButtonVisible());
            RandomWaiter.WaitSeconds(1, 3);
            ClickConfirmButton();
            wait.Until(e => IsHidden());
            RandomWaiter.WaitSeconds(1, 3);
            return IsHidden();
        }

        private void ClickConfirmButton()
        {
            var confirmHideButton = Driver.FindElements(ConfirmHideButtonSelector).FirstOrDefault();
            Actions actions = new Actions(Driver);
            actions.Click(confirmHideButton);
            actions.Perform();
        }

        private void ClickEnterButton()
        {
            var enterButton = Driver.FindElements(EnterButtonSelector).FirstOrDefault();
            Actions actions = new Actions(Driver);
            actions.Click(enterButton);
            actions.Perform();
        }

        private void ClickHideButton()
        {
            var hideButton = Driver.FindElements(HideButtonSelector).FirstOrDefault();
            Actions actions = new Actions(Driver);
            actions.Click(hideButton);
            actions.Perform();
        }

        private bool IsConfirmButtonVisible()
        {
            var confirmButton = Driver.FindElements(ConfirmHideButtonSelector).FirstOrDefault();
            return confirmButton != null;
        }

        private bool IsEntered()
        {
            var enterButton = Driver.FindElements(DeleteButtonSelector).First();
            var hidden = enterButton.GetAttribute("class").Contains("is-hidden");
            return !hidden;
        }

        private bool IsHidden()
        {
            var hideButton = Driver.FindElements(HideButtonSelector).FirstOrDefault();
            return hideButton == null;
        }
    }
}