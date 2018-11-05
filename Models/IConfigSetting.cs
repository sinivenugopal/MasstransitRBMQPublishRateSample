namespace MasstransitRBMQPublishRate
{
    using System.Collections.Generic;

    public interface IConfigSetting
    {
        List<KeyValuePair<string,string>> AppSetting { get; set; }
    }

}
