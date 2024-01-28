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
        private By HidePopupSelector => By.CssSelector("body div.popup popup--hide-games");

        public GiveawayPage(IWebDriver driver, string url) : base(driver)
        {
            Url = url;
            Driver
                .SwitchTo()
                .NewWindow(WindowType.Tab)
                .Navigate()
                .GoToUrl(Url);
        }

        public void ClickConfirmButton()
        {
            var hidePopup = Driver.FindElements(HidePopupSelector).FirstOrDefault();
            var confirmHideButton = Driver.FindElements(ConfirmHideButtonSelector).FirstOrDefault();
            Actions actions = new Actions(Driver);
            actions.Click(confirmHideButton);
            actions.Perform();
        }

        public void ClickHideButton()
        {
            var hideButton = Driver.FindElements(HideButtonSelector).FirstOrDefault();
            Actions actions = new Actions(Driver);
            actions.Click(hideButton);
            actions.Perform();
        }

        public void Dispose()
        {
            Driver.Close();
        }

        public void Enter()
        {
            var enterButton = Driver.FindElements(EnterButtonSelector).FirstOrDefault();
            Actions actions = new Actions(Driver);
            actions.Click(enterButton);
            actions.Perform();
        }

        public bool IsEntered()
        {
            var enterButton = Driver.FindElements(DeleteButtonSelector).First();
            var hidden = enterButton.GetAttribute("class").Contains("is-hidden");
            return !hidden;
        }

        public bool IsHidden()
        {
            var hideButton = Driver.FindElements(HideButtonSelector).FirstOrDefault();
            return hideButton == null;
        }

        //public bool IsConfirmHideButtonVisible()
        //{
        //}
        public bool PerformEnter()
        {
            Enter();
            WebDriverWait wait = new WebDriverWait(Driver, TimeSpan.FromSeconds(10));
            wait.Until(e => IsEntered());
            return IsEntered();
        }

        public bool PerformHide()
        {
            if (IsHidden())
                return true;
            ClickHideButton();
            WebDriverWait wait = new WebDriverWait(Driver, TimeSpan.FromSeconds(10));
            //wait.Until(ExpectedConditions)
            ClickConfirmButton();

            return IsHidden();
        }
    }
}