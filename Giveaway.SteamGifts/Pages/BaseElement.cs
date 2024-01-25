using AngleSharp.Dom;

using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;

namespace Giveaway.SteamGifts.Pages
{
    internal class BaseElement
    {
        public IWebDriver WebDriver { get; }
        public IWebElement WebElement { get; }

        public BaseElement(IWebDriver webDriver, IWebElement webElement)
        {
            WebDriver = webDriver;
            WebElement = webElement;
        }

        protected string GetTextBySelector(string selector)
        {
            var content = WebElement.FindElements(By.CssSelector(selector)).FirstOrDefault();

            return content?.Text ?? string.Empty;
        }

        protected string GetAttributeBySelector(string selector, string attribute)
        {
            var content = WebElement.FindElements(By.CssSelector(selector)).FirstOrDefault();
 
            return content?.GetAttribute(attribute) ?? string.Empty;
        }

        protected void ClickBySelector(string selector)
        {
            var elementBySelector = WebElement.FindElement(By.CssSelector(selector));
            Actions actions = new Actions(WebDriver);
            actions.Click(elementBySelector);
            actions.Perform();
        }

        protected void ClickBySelectorShift(string selector)
        {
            var elementBySelector = WebElement.FindElement(By.CssSelector(selector));
            Actions newTab = new Actions(WebDriver);
            newTab.KeyDown(Keys.LeftControl)
                .Click(elementBySelector).KeyUp(Keys.LeftControl)
                .Build()
                .Perform();
        }

        public virtual void Focus()
        {
            Actions actions = new Actions(WebDriver);
            actions.ScrollToElement(WebElement);
            actions.Perform();
        }

    }
}
