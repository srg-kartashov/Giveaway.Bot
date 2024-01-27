using Giveaway.SteamGifts.Commands;
using Giveaway.SteamGifts.Models;

namespace Giveaway.SteamGifts.Formatter
{
    internal class TelegramFormatter
    {
        public string FormatForLog(Statistic statistic)
        {
            return $"Успешно вступили: {statistic.Entered}, Не получилось вступить: {statistic.Failed}, Забраковали по фильтрам: {statistic.Skiped}, Скрыли: {statistic.Hidden}";
        }
        public string FormatForLog(UserData userData)
        {
            return $"👨‍💻 {userData.Name}\n📈 Уровень: {userData.Level}\n💰 Баланс: {userData.Points}";
        }
        public string FormatForLog(string massage, Exception exception)
        {
            return $"🛑 {massage}\n <pre>{exception.StackTrace?.Trim()}</pre>";
        }
    }
}
