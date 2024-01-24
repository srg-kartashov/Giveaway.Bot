using ConsoleTools;

using Giveaway.SteamGifts.Commands;
using Giveaway.SteamGifts.Models;

using Newtonsoft.Json;

using System.Text;

namespace Giveaway.SteamGifts
{

    internal class Program
    {
        private static string configPath = "Configuration.config";
        private static Configuration Configuration
        {
            get
            {
                if (File.Exists(configPath))
                {
                    var json = File.ReadAllText(configPath);
                    var obj = JsonConvert.DeserializeObject<Configuration>(json);
                    if (obj != null)
                    {
                        obj.Path = configPath;
                        return obj;
                    }
                    return new Configuration(configPath);
                }
                else
                {
                    return new Configuration(configPath);
                }
            }
        }


        static void Main(string[] args)
        {
            ConfigureConsole();
            if (IsHeadless(args))
            {
                BaseCommand startCommand = new StartCommand(Configuration, true);
                CommandBehavior commandBehavior = new CommandBehavior(startCommand);
                commandBehavior.Execute();
            }
            else
            {
                ShowConsoleMenu(args, Configuration);
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
              .Add("Изменить Telegram Bot", () => new TelegramEnterKeysCommand(config).Execute())
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
            Console.Title = "Giveaway.SteamGifts by Serhii Kartashov aka PtichiiPepel";
            Console.CursorVisible = false;
            Console.SetWindowSize(125, 34);
            Console.BufferWidth = Console.WindowWidth;
            Console.BufferHeight = short.MaxValue / 8;
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
    }
}