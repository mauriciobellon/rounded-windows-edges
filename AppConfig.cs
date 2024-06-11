using System;
using System.IO;
using Newtonsoft.Json;

namespace RoundedWindowsEdges
{
    public class AppConfig
    {
        public int CornerSize { get; set; } = 20;

        private static readonly string configFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.json");

        public static AppConfig LoadConfig()
        {
            if (File.Exists(configFilePath))
            {
                try
                {
                    string json = File.ReadAllText(configFilePath);
                    return JsonConvert.DeserializeObject<AppConfig>(json);
                }
                catch
                {
                    return new AppConfig();
                }
            }
            return new AppConfig();
        }

        public void SaveConfig()
        {
            try
            {
                string json = JsonConvert.SerializeObject(this, Newtonsoft.Json.Formatting.Indented);
                File.WriteAllText(configFilePath, json);
            }
            catch
            {
            }
        }
    }
}
