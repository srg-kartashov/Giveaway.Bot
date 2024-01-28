using System.Text;
using System.Text.RegularExpressions;

namespace Giveaway.SteamGifts.Extensions
{
    internal static class StringExtension
    {
        // TODO: Пересмотреть
        public static int? TryParseInt32(this string value)
        {
            StringBuilder stringBuilder = new StringBuilder();
            foreach(var ch in value)
                if(char.IsDigit(ch))
                    stringBuilder.Append(ch);
            var result = stringBuilder.ToString();
            if (string.IsNullOrEmpty(result))
                return null;
            return Convert.ToInt32(result);
        }
    }
}
