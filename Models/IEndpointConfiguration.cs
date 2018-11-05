namespace MasstransitRBMQPublishRate
{
    using System.Collections.Generic;
    using Unity;
    public interface IEndpointConfiguration
    {
        List<KeyValuePair<string, IUnityContainer>> ListenerInfo { get; set; }
    }
}
