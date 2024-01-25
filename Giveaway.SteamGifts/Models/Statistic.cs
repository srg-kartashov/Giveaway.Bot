namespace Giveaway.SteamGifts.Models
{
    internal class Statistic
    {
        public int AlreadyEntered { get; set; }
        public int Skiped { get; set; }
        public int Failed { get; set; }
        public int Entered { get; set; }

        public override string ToString()
        {
            return $"Успешно вступили: {Entered}, Не получилось вступить: {Failed}, Забраковали по фильтрам: {Skiped}";
        }
    }
}
