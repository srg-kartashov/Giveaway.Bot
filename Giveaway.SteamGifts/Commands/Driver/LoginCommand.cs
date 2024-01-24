using Giveaway.SteamGifts.Models;
using Giveaway.SteamGifts.Pages;

using NLog;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Giveaway.SteamGifts.Commands
{
    internal class LoginCommand : BaseCommand
    {
        private Logger Logger { get; set; } =  LogManager.GetCurrentClassLogger();
        public LoginCommand(Configuration configuration) : base(configuration)
        {
        }

        public override void Execute()
        {
            try
            {
                using (var webNavigator = new WebNavigator())
                {
                    webNavigator.Start();
                    var giveawayPage = webNavigator.GoToGiveawaysPage();
                    while (!giveawayPage.IsAuthorized())
                    {
                        Console.WriteLine("Вы не авторизиваны. Войдите пожалуйста в аккаунт");
                        Console.WriteLine("Повторная проверка через 10 секунд");
                        Thread.Sleep(10000);
                    }
                    webNavigator.Dispose();
                    Console.WriteLine("Авторизация прошла успешно");
                    Console.WriteLine("Press any key to continue...");
                    Console.ReadLine();
                }
            }
            catch (Exception ex)
            {

            }
        }
    }
}
