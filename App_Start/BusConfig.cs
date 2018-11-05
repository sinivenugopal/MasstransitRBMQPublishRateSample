namespace MasstransitRBMQPublishRate
{
    using System;
    using GreenPipes;
    using MassTransit;
    using MassTransit.RabbitMqTransport;
    using Unity;

    /// <summary>
    /// Abstract class for bus controls
    /// </summary>
    public abstract class BusConfig
    {
        private readonly string rabbitMQHost;
        private readonly string queueUserName;
        private readonly string queuePassword;
        private readonly bool hasCluster;
        private readonly string hostNames;
        private readonly bool hasSSL;
        private readonly string certificatePath;
        private readonly string certificatePassphrase;
        private readonly bool hasConsumer;
        private readonly int prefetchCount;

        private readonly bool hasRetry;
        private readonly int retryLimit;
        private readonly double initialInterval;
        private readonly double intervalIncrement;

        private IConfigSetting configSetting;
        private IUnityContainer container;

        /// <summary>
        /// Initializes a new instance of the <see cref="BusConfig"/> class.
        /// Injection constructor for busconfig class
        /// </summary>
        /// <param name="container"></param>
        public BusConfig(IUnityContainer container)
        {
            this.container = container;
            this.configSetting = container.Resolve<IConfigSetting>();

            this.rabbitMQHost = this.configSetting.AppSetting.Exists(x => x.Key.ToLower() == "host") ? this.configSetting.AppSetting.Find(x => x.Key.ToLower() == "host").Value : throw new NullReferenceException("Appsetting Key: 'host' missing");
            this.queueUserName = this.configSetting.AppSetting.Exists(x => x.Key.ToLower() == "queueusername") ? this.configSetting.AppSetting.Find(x => x.Key.ToLower() == "queueuserName").Value : throw new NullReferenceException("Appsetting Key: 'queueuserName' missing");
            this.queuePassword = this.configSetting.AppSetting.Exists(x => x.Key.ToLower() == "queuepassword") ? this.configSetting.AppSetting.Find(x => x.Key.ToLower() == "queuepassword").Value : throw new NullReferenceException("Appsetting Key: 'queuepassword' missing");

            this.hasCluster = this.configSetting.AppSetting.Exists(x => x.Key.ToLower() == "hascluster") ? Convert.ToBoolean(this.configSetting.AppSetting.Find(x => x.Key.ToLower() == "hascluster").Value) : throw new NullReferenceException("Appsetting Key: 'hascluster' missing");
            this.hostNames = this.configSetting.AppSetting.Exists(x => x.Key.ToLower() == "hostnames") ? this.configSetting.AppSetting.Find(x => x.Key.ToLower() == "hostnames").Value : throw new NullReferenceException("Appsetting Key: 'hostnames' missing");
            this.hasSSL = this.configSetting.AppSetting.Exists(x => x.Key.ToLower() == "hasssl") ? Convert.ToBoolean(this.configSetting.AppSetting.Find(x => x.Key.ToLower() == "hasssl").Value) : throw new NullReferenceException("Appsetting Key: 'hasssl' missing");
            this.certificatePath = this.configSetting.AppSetting.Exists(x => x.Key.ToLower() == "certificatepath") ? this.configSetting.AppSetting.Find(x => x.Key.ToLower() == "certificatepath").Value : throw new NullReferenceException("Appsetting Key: 'certificatepath' missing");
            this.certificatePassphrase = this.configSetting.AppSetting.Exists(x => x.Key.ToLower() == "certificatepassphrase") ? this.configSetting.AppSetting.Find(x => x.Key.ToLower() == "certificatepassphrase").Value : throw new NullReferenceException("Appsetting Key: 'certificatepassphrase' missing");
            this.hasConsumer = this.configSetting.AppSetting.Exists(x => x.Key.ToLower() == "hasconsumer") ? Convert.ToBoolean(this.configSetting.AppSetting.Find(x => x.Key.ToLower() == "hasconsumer").Value) : throw new NullReferenceException("Appsetting Key: 'hasconsumer' missing");
            this.prefetchCount = this.configSetting.AppSetting.Exists(x => x.Key.ToLower() == "prefetchcount") ? Convert.ToInt32(this.configSetting.AppSetting.Find(x => x.Key.ToLower() == "prefetchcount").Value) : 20;
            this.hasRetry = this.configSetting.AppSetting.Exists(x => x.Key.ToLower() == "hasretry") ? Convert.ToBoolean(this.configSetting.AppSetting.Find(x => x.Key.ToLower() == "hasretry").Value) : true;
            this.retryLimit = this.configSetting.AppSetting.Exists(x => x.Key.ToLower() == "retrylimit") ? Convert.ToInt32(this.configSetting.AppSetting.Find(x => x.Key.ToLower() == "retryLimit").Value) : 2;
            this.initialInterval = this.configSetting.AppSetting.Exists(x => x.Key.ToLower() == "initialinterval") ? Convert.ToDouble(this.configSetting.AppSetting.Find(x => x.Key.ToLower() == "initialinterval").Value) : 10.0D;
            this.intervalIncrement = this.configSetting.AppSetting.Exists(x => x.Key.ToLower() == "intervalincrement") ? Convert.ToDouble(this.configSetting.AppSetting.Find(x => x.Key.ToLower() == "intervalincrement").Value) : 1.0D;
        }

        /// <summary>
        /// Creates bus control for publishing messagedata to the queue
        /// </summary>
        /// <returns></returns>
        public IBusControl CreateBus()
        {
            return Bus.Factory.CreateUsingRabbitMq(x =>
            {
                x.Host(new Uri(this.rabbitMQHost), h =>
                {
                    h.Username(this.queueUserName);
                    h.Password(this.queuePassword);

                    if (Convert.ToBoolean(this.hasCluster))
                    {
                        h.UseCluster(c =>
                        {
                            if (!string.IsNullOrEmpty(this.hostNames))
                            {
                                string[] hostnames = this.hostNames.Split(';');
                                c.ClusterMembers = hostnames;
                            }
                        });
                    }
                    if (Convert.ToBoolean(this.hasSSL))
                    {
                        h.UseSsl(s =>
                        {
                            s.ServerName = System.Net.Dns.GetHostName();
                            s.CertificatePath = this.certificatePath;
                            s.CertificatePassphrase = this.certificatePassphrase;
                        });
                    }
                });
            });
        }

        /// <summary>
        /// Creates bus control instance for consuming the messagedata from the queue.
        /// Queuename and related container to be wrapped in the IEndpointConfiguration.
        /// </summary>
        /// <param name="endpointConfiguration"></param>
        /// <returns></returns>
        public IBusControl CreateBus(IEndpointConfiguration endpointConfiguration)
        {
            return Bus.Factory.CreateUsingRabbitMq(x =>
            {
                x.UseDelayedExchangeMessageScheduler();

                x.Host(new Uri(this.rabbitMQHost), h =>
                {
                    h.Username(this.queueUserName);
                    h.Password(this.queuePassword);

                    if (this.hasCluster)
                    {
                        h.UseCluster(c =>
                        {
                            if (!string.IsNullOrEmpty(this.hostNames))
                            {
                                string[] hostnames = this.hostNames.Split(';');
                                c.ClusterMembers = hostnames;
                            }
                        });
                    }
                    if (this.hasSSL)
                    {
                        h.UseSsl(s =>
                        {
                            s.ServerName = System.Net.Dns.GetHostName();
                            s.CertificatePath = this.certificatePath;
                            s.CertificatePassphrase = this.certificatePassphrase;
                        });
                    }
                });

                if (this.hasConsumer)
                {
                    foreach (var listener in endpointConfiguration.ListenerInfo)
                    {
                        if (this.prefetchCount > 0)
                        {
                            x.PrefetchCount = (ushort)this.prefetchCount;
                        }

                        x.ReceiveEndpoint(listener.Key, ec =>
                        {
                            if (this.hasRetry)
                            {
                                ec.UseScheduledRedelivery(r => r.Incremental(
                                    this.retryLimit,
                                    TimeSpan.FromSeconds(this.initialInterval),
                                    TimeSpan.FromSeconds(this.intervalIncrement)));
                            }

                            ec.LoadFrom(listener.Value);
                        });
                    }
                }
            });
        }

        /// <summary>
        /// Abstract method to configure simple consumers or listeners.
        /// </summary>
        /// <returns></returns>
        public abstract Action<IRabbitMqReceiveEndpointConfigurator> Configure();
    }
}
