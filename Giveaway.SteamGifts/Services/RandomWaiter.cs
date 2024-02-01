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

        public void WaitHours(int hoursFrom, int hoursTo) => WaitMinutes(hoursFrom * 60, hoursTo * 60);

        public void WaitHours(int hours) => WaitHours(0, hours);

        public void WaitMinutes(int minutesFrom, int minutesTo) => WaitSeconds(minutesFrom * 60, minutesTo * 60);

        public void WaitMinutes(int minutes) => WaitMinutes(0, minutes);

        public void WaitSeconds(int secondsFrom, int secondsTo) => WaitMilliseconds(secondsFrom * 1000, secondsTo * 1000);

        public void WaitSeconds(int seconds) => WaitSeconds(0, seconds);

        private string GetMessageForLogger(int milliseconds)
        {
            TimeSpan timeSpan = TimeSpan.FromMilliseconds(milliseconds);
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("Wait for ");
            if (timeSpan.Hours != 0)
                stringBuilder.Append($"{timeSpan.Hours} hours, ");
            if (timeSpan.Minutes != 0)
                stringBuilder.Append($"{timeSpan.Minutes} minutes, ");
            if (timeSpan.Seconds != 0)
                stringBuilder.Append($"{timeSpan.Seconds} seconds");
            return stringBuilder.ToString();
        }

        private void WaitMilliseconds(int from, int to)
        {
            var value = Random.Next(from, to);
            Logger.Trace(GetMessageForLogger(value));
            TimeSpan timeSpan = TimeSpan.FromMilliseconds(value);
            Thread.Sleep(value);
        }
    }
}