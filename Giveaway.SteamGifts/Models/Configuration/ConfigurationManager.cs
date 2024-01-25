using Newtonsoft.Json;

namespace Giveaway.SteamGifts.Models
{
    internal class ConfigurationManager
    {
        public string Path { get; set; }
        private Configuration Configuration => GetConfiguration();


        public ConfigurationManager(string path)
        {
            Path = path;
        }

        private Configuration GetConfiguration()
        {
            var jsonConfig = File.ReadAllText(Path);
            var config = JsonConvert.DeserializeObject<Configuration>(jsonConfig);
            return config ?? new Configuration();
        }

        public void UpdateConfiguration()
        {
            var json = JsonConvert.SerializeObject(Configuration);
            File.WriteAllText(Path, json);
        }
        
    }
}
