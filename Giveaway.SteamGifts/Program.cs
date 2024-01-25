using ConsoleTools;

using Giveaway.SteamGifts.Commands;
using Giveaway.SteamGifts.Models;


//using Microsoft.Extensions.Configuration;

using Newtonsoft.Json;

using NLog;

using System.Text;

namespace Giveaway.SteamGifts
{

    internal class Program
    {
        private static ILogger Logger = LogManager.GetCurrentClassLogger();
        private const string ConfigFilePath = "Configuration.json";

        static void Main(string[] args)
        {
            ConfigureConsole();
            bool isHeadless = IsHeadless(args);
            try
            {
                Configuration Configuration = LoadConfiguration();
                if (isHeadless)
                {
                    new StartCommand(Configuration, true).Execute();
                }
                else
                {
                    ShowConsoleMenu(args, Configuration);
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex, ex.Message);
                if (!isHeadless)
                {
                    Console.WriteLine("Press any key to close application...");
                    Console.ReadLine();
                }
            }
        }

        public static void ShowConsoleMenu(string[] args, Configuration config)
        {
            var driverSettings = new ConsoleMenu(args, level: 2)
               .Add("Авторизация в SteamGifts", () => new LoginCommand(config).Execute())
               .Add("Установить AutoJoin", () => new InstallAutoJoinPluginCommand(config).Execute())
               .Add("Удалить профиль ChromeDriver", () => new DriverRemoveProfileCommand(config).Execute())
               .Add("Назад", ConsoleMenu.Close)
               .Configure(config =>
               {
                   config.Selector = "--> ";
                   config.Title = "Браузер";
                   config.SelectedItemBackgroundColor = ConsoleColor.Magenta;
                   config.EnableWriteTitle = false;
                   config.EnableBreadcrumb = true;
                   config.WriteBreadcrumbAction = titles => WriteLogo(string.Join(" / ", titles));
               });



            var telegramSettings = new ConsoleMenu(args, level: 2)
              .Add("Информация", () => new TelegramShowKeysCommand(config).Execute())
              .Add("Изменить Telegram Bot", () => new TelegramEnterKeysCommand(config, ConfigFilePath).Execute())
              .Add("Проверка Telegram Bot", () => new TelegramTrySendCommand(config).Execute())
              .Add("Назад", ConsoleMenu.Close)
              .Configure(config =>
              {
                  config.Selector = "--> ";
                  config.Title = "Telegram Bot";
                  config.SelectedItemBackgroundColor = ConsoleColor.Magenta;
                  config.EnableWriteTitle = false;
                  config.EnableBreadcrumb = true;
                  config.WriteBreadcrumbAction = titles => WriteLogo(string.Join(" / ", titles));
              });

            var settings = new ConsoleMenu(args, level: 1)
              .Add("Telegram Bot", telegramSettings.Show)
              .Add("Браузер", driverSettings.Show)
              .Add("Назад", ConsoleMenu.Close)
              .Configure(config =>
              {
                  config.Selector = "--> ";
                  config.Title = "Настройки";
                  config.SelectedItemBackgroundColor = ConsoleColor.Magenta;
                  config.EnableWriteTitle = false;
                  config.EnableBreadcrumb = true;
                  config.WriteBreadcrumbAction = titles => WriteLogo(string.Join(" / ", titles));
              });

            var menu = new ConsoleMenu(args, level: 0)
              .Add("Запуск", () => new StartCommand(config, false).Execute())
              .Add("Запуск [Headless]", () => new StartCommand(config, true).Execute())
              .Add("Настройки", settings.Show)
              .Add("Статистика", () => new StatisticCommand(config).Execute())
              .Add("Выход", () => Environment.Exit(0))
              .Configure(config =>
              {
                  config.Selector = "--> ";
                  config.Title = "Главное меню";
                  config.SelectedItemBackgroundColor = ConsoleColor.Magenta;
                  config.EnableWriteTitle = false;
                  config.EnableBreadcrumb = true;
                  config.WriteBreadcrumbAction = titles => WriteLogo(string.Join(" / ", titles));
              });

            menu.Show();
        }

        public static bool IsHeadless(string[] args)
        {
            return args.Contains("-headless");
        }

        public static void ConfigureConsole()
        {
            Console.Title = "Giveaway.SteamGifts";
            Console.CursorVisible = false;
            Console.SetWindowSize(125, 34);
            try
            {
                Console.BufferWidth = Console.WindowWidth;
                Console.BufferHeight = short.MaxValue / 8;
            }
            catch { }
        }

        public static void WriteLogo(string title)
        {
            StringBuilder headerBuilder = new StringBuilder();
            var logo = File.ReadAllText("Images\\logo.txt");
            headerBuilder.AppendLine(logo);
            headerBuilder.AppendLine();
            headerBuilder.AppendLine(title);
            headerBuilder.AppendLine();
            Console.WriteLine(headerBuilder.ToString());
        }

        public static Configuration LoadConfiguration()
        {
            try
            {
                Configuration? configuration = null;
                if (File.Exists(ConfigFilePath))
                {
                    string json = File.ReadAllText(ConfigFilePath);
                    configuration = JsonConvert.DeserializeObject<Configuration>(json);
                }

                return configuration ?? throw new Exception("Ошибка во время чтения файла конфигурации");
            }
            catch
            {
                Logger.Error("Ошибка во время считывания конфигурации");
                throw;
            }
        }

    }
}