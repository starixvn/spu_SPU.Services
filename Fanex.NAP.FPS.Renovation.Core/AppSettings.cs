namespace Fanex.NAP.FPS.Renovation.Core
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Linq;

    public static class AppSettings
    {
        public static int Environment
        {
            get
            {

                var environment = ConfigurationManager.AppSettings["Environment"];

                switch (environment)
                {
                    case "1":
                        return Convert.ToInt16(Core.Environment.LOCAL);
                    case "2":
                        return Convert.ToInt16(Core.Environment.UAT);
                    default:
                        return Convert.ToInt16(Core.Environment.PRODUCTION);
                }
            }
        }

        public static string Host => ConfigurationManager.AppSettings["Queue-Host"];

        public static string Post => ConfigurationManager.AppSettings["Queue-Port"];

        public static string VirtualHost => ConfigurationManager.AppSettings["Queue-VirtualHost"];

        public static string UserName => ConfigurationManager.AppSettings["Queue-UserName"];

        public static string Password => ConfigurationManager.AppSettings["Queue-Password"];

        public static int PrefetchCount => Convert.ToInt16(ConfigurationManager.AppSettings["Queue-PrefetchCount"]);

        public static string Exchange => ConfigurationManager.AppSettings["Queue-Exchange"];

        public static string QueueName => ConfigurationManager.AppSettings["Queue-Name"];

        public static string RoutingKey => ConfigurationManager.AppSettings["Queue-RoutingKey"];

        public static string AppDataSetting => ConfigurationManager.AppSettings["AppData-Path"];

        public static string FanexLogCategory => ConfigurationManager.AppSettings["Log-SentryLogging-Category"];

        public static string SentryLogging => ConfigurationManager.AppSettings["Log-SentryLogging-Config"];

        public static int Interval => Convert.ToInt32(ConfigurationManager.AppSettings["Interval"]);

        public static int TransBatch => Convert.ToInt32(ConfigurationManager.AppSettings["Trans-Batch"]);

        public static string HostPathForTransaction => ConfigurationManager.AppSettings["SelfHost-Url-Trans"];

        public static string HostPathForDevice=> ConfigurationManager.AppSettings["SelfHost-Url-Device"];

        public static int SizeOfDevice => Convert.ToInt32(ConfigurationManager.AppSettings["Device-Size"]);
        
        public static int SizeOfAssociation => Convert.ToInt32(ConfigurationManager.AppSettings["Association-Size"]);
        
        public static int SizeOfTransaction => Convert.ToInt32(ConfigurationManager.AppSettings["Transaction-Size"]);
        
        public static int SizeOfAccount => Convert.ToInt32(ConfigurationManager.AppSettings["Account-Size"]);
        
        public static int SizeOfCustomer => Convert.ToInt32(ConfigurationManager.AppSettings["Customer-Size"]);
        
        public static int SizeOfIpRange => Convert.ToInt32(ConfigurationManager.AppSettings["IpRange-Size"]);
        
        public static int SizeOfUserAgent => Convert.ToInt32(ConfigurationManager.AppSettings["UserAgent-Size"]);

        public static string SSHUser => ConfigurationManager.AppSettings["ssh-User"];

        public static string SSHPassword => ConfigurationManager.AppSettings["ssh-Password"];

        public static string SSHHost => ConfigurationManager.AppSettings["ssh-Host"];

        public static int SSHPort => Convert.ToInt32(ConfigurationManager.AppSettings["ssh-Port"]);

        public static string SSHDestinationPath => ConfigurationManager.AppSettings["ssh-DestinationPath"];
    }
}
