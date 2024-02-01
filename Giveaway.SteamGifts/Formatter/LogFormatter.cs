using Giveaway.SteamGifts.Models;

using System.Text;

namespace Giveaway.SteamGifts.Formatter
{
    internal class LogFormatter
    {
        public string FormatForLog(Statistic statistic)
        {
            return $"Успешно вступили: {statistic.Joined}, Не получилось вступить: {statistic.Failed}, Забраковали по фильтрам: {statistic.Skiped}, Скрыли: {statistic.Hidden}, Не удалось скрыть: {statistic.FailedHidden}";
        }

        public string FormatForLog(UserData userData)
        {
            return "Имя пользователя: " + userData.Name + " Уровень: " + userData.Level + " Баланс: " + userData.Points;
        }

        public string FormatForLog(GiveawayData giveawayData, GiveawayAction action)
        {
            StringBuilder loggerMessage = new StringBuilder();
            loggerMessage.Append($"[{giveawayData.Reviews} - {giveawayData.Raiting}%]".PadRight(16));
            loggerMessage.Append($"Name: {giveawayData.Name}; ");
            loggerMessage.Append($"Points: {giveawayData.Points}; ");
            loggerMessage.Append($"Action: ");
            loggerMessage.Append(Enum.GetName(typeof(GiveawayAction), action));
            if (giveawayData.IsCollection) loggerMessage.Append(" IsCollection;");
            return loggerMessage.ToString();
        }
    }
}