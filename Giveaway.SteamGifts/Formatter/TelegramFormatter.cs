﻿using Giveaway.SteamGifts.Models;

using System.Text;

namespace Giveaway.SteamGifts.Formatter
{
    internal class TelegramFormatter
    {
        public string FormatForLog(Statistic statistic)
        {
            return $"✅Успешно вступили: {statistic.Joined}\n⚠️Не получилось вступить: {statistic.Failed}\n⏭️Пропустили: {statistic.Skiped}\n👁Скрыли: {statistic.Hidden}\n❗👁Не удалось скрыть: {statistic.FailedHidden}";
        }

        public string FormatForLog(UserData userData)
        {
            return $"👨‍💻 {userData.Name}\n📈 Уровень: {userData.Level}\n💰 Баланс: {userData.Points}";
        }

        public string FormatForLog(string massage, Exception exception)
        {
            return $"🛑 {massage}\n <pre>{exception.StackTrace?.Trim()}</pre>";
        }

        public string FormatForLog(GiveawayData giveawayData, GiveawayAction action)
        {
            StringBuilder telegramMessage = new StringBuilder();
            switch (action)
            {
                case GiveawayAction.Failed:
                    telegramMessage.Append("⚠️");
                    break;

                case GiveawayAction.Join:
                    telegramMessage.Append("✅");
                    break;

                case GiveawayAction.Hide:
                    telegramMessage.Append("👁");
                    break;

                case GiveawayAction.FailedHide:
                    telegramMessage.Append("❗👁");
                    break;
            }

            telegramMessage.Append($" <a href =\"{giveawayData.SteamUrl}\">{giveawayData.Name}</a>");
            telegramMessage.Append($" [{giveawayData.Reviews} - {giveawayData.Raiting}%] ");
            telegramMessage.Append($" <a href =\"{giveawayData.GiveawayGiftUrl}\">🌐</a>");
            return telegramMessage.ToString();
        }
    }
}