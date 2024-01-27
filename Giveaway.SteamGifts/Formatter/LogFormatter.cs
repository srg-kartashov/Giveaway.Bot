using Giveaway.SteamGifts.Models;
using Giveaway.SteamGifts.Pages.Giveaways;

namespace Giveaway.SteamGifts.Formatter
{
    internal class LogFormatter
    {
        public string FormatForLog(Statistic statistic)
        {
            return $"Успешно вступили: {statistic.Entered}, Не получилось вступить: {statistic.Failed}, Забраковали по фильтрам: {statistic.Skiped}, Скрыли: {statistic.Hidden}";
        }

        public string FormatForLog(UserData userData)
        {
            return "Имя пользователя: " + userData.Name + " Уровень: " + userData.Level + " Баланс: " + userData.Points;
        }
    }
}
