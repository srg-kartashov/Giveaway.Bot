using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Giveaway.SteamGifts.Pages.Giveaways
{
    internal class GiveawayPage : BasePage, IDisposable
    {
        public string Url { get; }

        private By EnterButtonSelector => By.CssSelector("form div[data-do='entry_insert'].sidebar__entry-insert");
        private By DeleteButtonSelector => By.CssSelector("form div[data-do='entry_delete'].sidebar__entry-delete");
        private By HideButtonSelector => By.CssSelector("div.featured__heading i.featured__giveaway__hide");
        private By HidePopupSelector => By.CssSelector("body div.popup popup--hide-games");
        private By ConfirmHideButtonSelector => By.CssSelector("div.form__submit-button");

        public GiveawayPage(IWebDriver driver, string url) : base(driver)
        {
            Url = url;
            Driver
                .SwitchTo()
                .NewWindow(WindowType.Tab)
                .Navigate()
                .GoToUrl(Url);           
        }

        public void Enter()
        {
            var enterButton = Driver.FindElements(EnterButtonSelector).FirstOrDefault();
            Actions actions = new Actions(Driver);
            actions.Click(enterButton);
            actions.Perform();
        }

        public bool IsHidden()
        {
            var hideButton = Driver.FindElements(HideButtonSelector).FirstOrDefault();
            return hideButton == null;
        }

        public void ClickHideButton()
        {
            var hideButton = Driver.FindElements(HideButtonSelector).FirstOrDefault();
            Actions actions = new Actions(Driver);
            actions.Click(hideButton);
            actions.Perform();
        }

        public void ClickConfirmButton()
        {
            var hidePopup = Driver.FindElements(HidePopupSelector).FirstOrDefault();
            var confirmHideButton = Driver.FindElements(ConfirmHideButtonSelector).FirstOrDefault();
            Actions actions = new Actions(Driver);
            actions.Click(confirmHideButton);
            actions.Perform();
        }

        public bool IsEntered()
        {
            var enterButton = Driver.FindElements(DeleteButtonSelector).First();
            var hidden = enterButton.GetAttribute("class").Contains("is-hidden");
            return !hidden;
        }

        public void Dispose()
        {
            Driver.Close();
        }
    }
}
