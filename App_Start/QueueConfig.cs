namespace MasstransitRBMQPublishRate
{
    using MassTransit;
    using MassTransit.RabbitMqTransport;
    using System;
    using Unity;

    public class QueueConfig : BusConfig
    {
        public static IBusControl busControl;

        public QueueConfig(IUnityContainer baseContainer)
            : base(baseContainer)
        {
        }

        public void StartBus()
        {
            busControl = this.CreateBus();
            busControl.Start();
        }

        public void StartBus(IEndpointConfiguration endpointConfiguration)
        {
            busControl = this.CreateBus();
            busControl.Start();
        }

        public override Action<IRabbitMqReceiveEndpointConfigurator> Configure()
        {
            throw new NotImplementedException();
        }
    }
}