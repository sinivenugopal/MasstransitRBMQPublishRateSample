using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Unity;
using Unity.Lifetime;

namespace MasstransitRBMQPublishRate
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            try
            {
                var container = new UnityContainer();
                ConfigSetting configSetting = this.SetAppSettingValues();
                container.RegisterInstance<IConfigSetting>(configSetting, new ExternallyControlledLifetimeManager());
                
                QueueConfig queueConfiguration = new QueueConfig(container);
                queueConfiguration.StartBus();
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private ConfigSetting SetAppSettingValues()
        {
            try
            {
                ConfigSetting settings = new ConfigSetting();
                List<KeyValuePair<string, string>> appsettings = new List<KeyValuePair<string, string>>();
                appsettings.Add(new KeyValuePair<string, string>("host", this.GetConfigValue("Host")));
                appsettings.Add(new KeyValuePair<string, string>("queueusername", this.GetConfigValue("QueueUserName")));
                appsettings.Add(new KeyValuePair<string, string>("queuepassword", this.GetConfigValue("QueuePassword")));
                appsettings.Add(new KeyValuePair<string, string>("hascluster", this.GetConfigValue("hasCluster")));
                appsettings.Add(new KeyValuePair<string, string>("hasssl", this.GetConfigValue("hasSSL")));
                appsettings.Add(new KeyValuePair<string, string>("hasconsumer", this.GetConfigValue("hasConsumer")));
                appsettings.Add(new KeyValuePair<string, string>("hostNames", this.GetConfigValue("hostNames")));
                appsettings.Add(new KeyValuePair<string, string>("certificatepath", this.GetConfigValue("CertificatePath")));
                appsettings.Add(new KeyValuePair<string, string>("certificatepassphrase", this.GetConfigValue("CertificatePassphrase")));
                appsettings.Add(new KeyValuePair<string, string>("hasretry", this.GetConfigValue("hasRetry")));
                appsettings.Add(new KeyValuePair<string, string>("prefetchcount", this.GetConfigValue("PrefetchCount")));
                appsettings.Add(new KeyValuePair<string, string>("retrylimit", this.GetConfigValue("RetryLimit")));
                appsettings.Add(new KeyValuePair<string, string>("initialinterval", this.GetConfigValue("InitialInterval")));
                appsettings.Add(new KeyValuePair<string, string>("intervalincrement", this.GetConfigValue("IntervalIncrement")));
                settings.AppSetting = appsettings;

                return settings;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private string GetConfigValue(string key)
        {
            if (string.IsNullOrWhiteSpace(ConfigurationManager.AppSettings[key]))
            {
                throw new ConfigurationErrorsException("Missing Appsetting: " + key);
            }
            else
            {
                return ConfigurationManager.AppSettings[key];
            }
        }

    }
}
