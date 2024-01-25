using NLog;

using System.Text;

namespace Giveaway.SteamGifts.Services
{
    internal class RandomWaiter
    {
        private ILogger Logger => LogManager.GetCurrentClassLogger();
        private Random Random { get; }
        public RandomWaiter()
        {
            Random = new Random();
        }

        public void WaitSeconds(int secondsFrom, int secondsTo) => WaitMilliseconds(secondsFrom * 1000, secondsTo * 1000);
        public void WaitMinutes(int minutesFrom, int minutesTo) => WaitSeconds(minutesFrom * 60, minutesTo * 60);
        public void WaitHours(int hoursFrom, int hoursTo) => WaitMinutes(hoursFrom * 60, hoursTo * 60);
        public void WaitSeconds(int seconds) => WaitSeconds(0, seconds);
        public void WaitMinutes(int minutes) => WaitMinutes(0, minutes);
        public void WaitHours(int hours) => WaitHours(0, hours);

        private void WaitMilliseconds(int from, int to)
        {
            var value = Random.Next(from, to);
            Logger.Info(GetMessageForLogger(value));
            TimeSpan timeSpan = TimeSpan.FromMilliseconds(value);
            Thread.Sleep(value);
        }

        private string GetMessageForLogger(int milliseconds)
        {
            TimeSpan timeSpan = TimeSpan.FromMilliseconds(milliseconds);
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("Ожидаем ");
            if (timeSpan.Hours != 0)
                stringBuilder.Append($"{timeSpan.Hours} часов, ");
            if (timeSpan.Minutes != 0)
                stringBuilder.Append($"{timeSpan.Minutes} минут, ");
            if (timeSpan.Seconds != 0)
                stringBuilder.Append($"{timeSpan.Seconds} секунд");
            return stringBuilder.ToString();
        }
    }
}
