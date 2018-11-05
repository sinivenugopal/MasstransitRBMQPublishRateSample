namespace MasstransitRBMQPublishRate
{
    using System.Collections.Generic;

    public class ConfigSetting : IConfigSetting
    {
        public List<KeyValuePair<string, string>> AppSetting { get; set; }
    }
}
