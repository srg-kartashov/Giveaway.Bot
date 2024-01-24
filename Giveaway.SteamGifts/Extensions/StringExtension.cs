using System.Text.RegularExpressions;

namespace Giveaway.SteamGifts.Extensions
{
    internal static class StringExtension
    {
        // TODO: Пересмотреть
        public static int ParseInteger(this string str)
        {
            var result = Regex.Match(str, @"\d+").Value;
            return Convert.ToInt32(result);
        }
    }
}
