namespace MasstransitRBMQPublishRate
{
    using System.Collections.Generic;
    using Unity;

    public class EndpointConfiguration : IEndpointConfiguration
    {
        public List<KeyValuePair<string, IUnityContainer>> ListenerInfo { get; set; }
    }
}
