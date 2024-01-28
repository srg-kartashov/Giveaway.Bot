using Giveaway.SteamGifts.Models;
using Giveaway.SteamGifts.Pages.SteamGift;
using Giveaway.SteamGifts.Pages.SteamGift.Elements;

using System;
using System.Text;

namespace Giveaway.SteamGifts.Formatter
{
    internal class LogFormatter
    {
        public string FormatForLog(Statistic statistic)
        {
            return $"Успешно вступили: {statistic.Entered}, Не получилось вступить: {statistic.Failed}, Забраковали по фильтрам: {statistic.Skiped}, Скрыли: {statistic.Hidden}, Не удалось скрыть: {statistic.FailedHidden}";
        }

        public string FormatForLog(UserData userData)
        {
            return "Имя пользователя: " + userData.Name + " Уровень: " + userData.Level + " Баланс: " + userData.Points;
        }

        public string FormatForLog(GameGiveaway game, GiveawayAction action)
        {
            StringBuilder loggerMessage = new StringBuilder();
            loggerMessage.Append($"[{game.Reviews} - {game.Raiting}%]".PadRight(16));
            loggerMessage.Append($"Name: {game.Name}; ");
            loggerMessage.Append($"Points: {game.Points}; ");
            loggerMessage.Append($"Action: ");
            loggerMessage.Append(Enum.GetName(typeof(GiveawayAction), action));
            if (game.IsCollection) loggerMessage.Append(" IsCollection;");
            return loggerMessage.ToString();
        }
    }
}
