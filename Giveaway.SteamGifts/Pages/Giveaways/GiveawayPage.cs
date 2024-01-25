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
