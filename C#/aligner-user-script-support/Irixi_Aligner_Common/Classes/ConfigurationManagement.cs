using Newtonsoft.Json;
using System.IO;

namespace Irixi_Aligner_Common.Classes
{
    public class ConfigurationManagement
    {
        string json_config = string.Empty;
        ConfigSystem _config = null;

        public ConfigurationManagement()
        {
            StreamReader reader = File.OpenText("appconfig.json");
            json_config = reader.ReadToEnd();
            _config = JsonConvert.DeserializeObject<ConfigSystem>(json_config);
        }

        public ConfigSystem Configuration
        {
            get
            {
                return _config;
            }
        }
    }

    public class ConfigAxis
    {
        public string Name { set; get; }
        public int SoftLLP { get; set; }
        public int SoftULP { get; set; }
    }

    public class ConfigStage
    {
        public string Name { set; get; }
        public string COM { set; get; }
        public bool Enabled { set; get; }
        public ConfigAxis X;
        public ConfigAxis Y;
        public ConfigAxis Z;
        public ConfigAxis Roll;
        public ConfigAxis Yaw;
        public ConfigAxis Pitch;

    }

    public class ConfigStages
    {
        public ConfigStage Vgroove { set; get; }
        public ConfigStage Lens { set; get; }
        public ConfigStage POD { set; get; }
        public ConfigStage[] Thorlabs { set; get; }
    }

    public class ConfigSystem
    {
        public bool LogEnabled { set; get; }
        public ConfigStages Stages { set; get; }
    }
}
