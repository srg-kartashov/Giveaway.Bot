using Giveaway.SteamGifts.Models;

using Newtonsoft.Json;

using NLog;

namespace Giveaway.SteamGifts.Commands
{
    internal class TelegramEnterKeysCommand : BaseCommand
    {
        public ILogger Logger => LogManager.GetCurrentClassLogger();

        public string ConfigurationPath { get; }

        public TelegramEnterKeysCommand(Configuration configuration, string configurationPath) : base(configuration)
        {
            ConfigurationPath = configurationPath;
        }

        public override void Execute()
        {
            try
            {
                Console.CursorVisible = true;
                Console.Write("Введите Telegram BotToken (https://t.me/BotFather): ");
                var botToken = Console.ReadLine();
                if (botToken != null)
                {
                    Configuration.Telegram.BotToken = botToken;
                }
                Console.Write("Введите Telegram СhatId (https://t.me/getmyid_bot): ");
                var chatID = Console.ReadLine();
                if (chatID != null)
                {
                    Configuration.Telegram.ChatId = chatID;
                }
                UpdateConfig(Configuration, ConfigurationPath);
                Console.WriteLine("Ключи обновлены успешно");
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                Console.WriteLine("Ошибка во время ввода ключей");
            }
            finally
            {
                Console.CursorVisible = false;
                Console.WriteLine("Press any key to continue...");
                Console.ReadLine();
            }
        }

        private void UpdateConfig(Configuration config, string path)
        {
            var json = JsonConvert.SerializeObject(config);
            File.WriteAllText(path, json);
        }
    }
}