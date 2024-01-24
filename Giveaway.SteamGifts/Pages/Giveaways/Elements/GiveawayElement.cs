using Giveaway.SteamGifts.Extensions;

using OpenQA.Selenium;

using System.Text.RegularExpressions;

namespace Giveaway.SteamGifts.Pages.Giveaways.Elements
{
    internal class GiveawayElement : BaseElement
    {
        public string GameName => GetTextBySelector("a.giveaway__heading__name");
        public string GameUrl => GetAttributeBySelector("a.giveaway__icon", "href");
        public string GiveawayUrl => GetTextBySelector("a.giveaway__heading__name");
        public bool NoPoints => GetAttributeBySelector("input.btnSingle", "walkstate").Equals("no-points");
        public bool AlreadyEntered => GetAttributeBySelector("input.btnSingle", "walkstate").Equals("leave");
        public bool IsCollection => GameUrl.Contains("sub");
        public int ApplicationId => ParseApplicationId(GameUrl);
        public int Points => GetTextBySelector("span.giveaway__heading__thin").ParseInteger();

        public GiveawayElement(IWebDriver webDriver, IWebElement webElement) : base(webDriver, webElement)
        {
        }

        public void Enter() => ClickBySelector("input.btnSingle");

        public void Hide() => ClickBySelector("i.giveaway__hide");

        // TODO: Пересмотреть
        private int ParseApplicationId(string url)
        {
            string gamePattern = @"store.steampowered.com\/app\/(\d+)\/";
            string collectionPattern = @"store.steampowered.com\/sub\/(\d+)\/";
            MatchCollection matches;
            if (Regex.IsMatch(url, gamePattern))
            {
                matches = Regex.Matches(url, gamePattern);
            }
            else if (Regex.IsMatch(url, collectionPattern))
            {
                matches = Regex.Matches(url, collectionPattern);
            }
            else
                throw new InvalidDataException();


            var result = matches.First().Groups.Values.Last().Value;
            return Convert.ToInt32(result);
        }




    }
}
