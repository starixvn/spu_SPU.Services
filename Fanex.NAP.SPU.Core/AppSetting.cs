namespace Fanex.NAP.SPU.Core
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Linq;

    public static class AppSetting
    {
        public static int Environment
        {
            get
            {

                var environment = ConfigurationManager.AppSettings["Environment"];

                switch (environment)
                {
                    case "1":
                        return Convert.ToInt16(Model.Environment.Local);
                    case "2":
                        return Convert.ToInt16(Model.Environment.UAT);
                    default:
                        return Convert.ToInt16(Model.Environment.Production);
                }
            }
        }

        public static string AppData => ConfigurationManager.AppSettings["AppData-Path"];

        public static string Host => ConfigurationManager.AppSettings["Queue-Host"];

        public static string Post => ConfigurationManager.AppSettings["Queue-Port"];

        public static string VirtualHost => ConfigurationManager.AppSettings["Queue-VirtualHost"];

        public static string UserName => ConfigurationManager.AppSettings["Queue-UserName"];

        public static string Password => ConfigurationManager.AppSettings["Queue-Password"];

        public static int PrefetchCount => Convert.ToInt16(ConfigurationManager.AppSettings["Queue-PrefetchCount"]);

        public static string Exchange => ConfigurationManager.AppSettings["Queue-Exchange"];

        public static string FanexLogCategory => ConfigurationManager.AppSettings["Fanex.Log.Category"];

        public static string FanexLogRabbitMQHost => ConfigurationManager.AppSettings["Fanex.Log.RabbitMQ.Host"];

        public static string HeartBeatUrl => ConfigurationManager.AppSettings["HeartBeat-Path"];

        public static string HeartBeatPort => ConfigurationManager.AppSettings["HeartBeat-Port"];

        public static string GroupCustomerPath => ConfigurationManager.AppSettings["MS-WebAPI-GroupCustomer"];

        public static string CustomerEvidenceApi => ConfigurationManager.AppSettings["FPS-WebAPI-Get-CustomerEvidence"];

        public static string SentryLogging => ConfigurationManager.AppSettings["Log-SentryLogging-Config"];

        public static int FraudCaseId => Convert.ToInt32(ConfigurationManager.AppSettings["FraudCase.FraudCaseId"].ToString());

        public static string FraudCaseName => ConfigurationManager.AppSettings["FraudCase.FraudCaseName"];

        public static int FraudCaseInterval => Convert.ToInt32(ConfigurationManager.AppSettings["FraudCase.FraudCaseInterval"].ToString());

        public static string FraudCaseDesc => ConfigurationManager.AppSettings["FraudCase.FraudCaseDesc"];

        public static bool IsActive => Convert.ToBoolean(ConfigurationManager.AppSettings["FraudCase.IsActive"].ToString());

        public static bool IsTest => Convert.ToBoolean(ConfigurationManager.AppSettings["FraudCase.IsTest"].ToString());

        public static List<int> IgnoreCase => ConfigurationManager.AppSettings["Ignore-fraudcase"].Split(',').Select(Int32.Parse).ToList();

        public static string LetstalkTarget => ConfigurationManager.AppSettings["Letstalk-Target"];

        public static string LetstalkGroup => ConfigurationManager.AppSettings["Letstalk-Group"];

        public static string SiteIds => ConfigurationManager.AppSettings["SiteIds"];

        public static string SuperIds => ConfigurationManager.AppSettings["SuperIds"];

        public static int DummyUser
        {
            get
            {
                string dummyUser = ConfigurationManager.AppSettings["DummyUser"];

                if (!string.IsNullOrEmpty(dummyUser))
                {
                    return Convert.ToInt32(dummyUser);
                }

                return 0;
            }
        }

        public static string IP => ConfigurationManager.AppSettings["IP"];

        public static int PunterChunkSize
        {
            get
            {
                string count = ConfigurationManager.AppSettings["PunterChunkSize"];

                if (!string.IsNullOrWhiteSpace(count))
                {
                    return Convert.ToInt32(count);
                }

                return 1000;
            }
        }

        public static int NumOfMonth
        {
            get
            {
                var value = ConfigurationManager.AppSettings["NumOfMonth"];

                if (!string.IsNullOrWhiteSpace(value))
                {
                    return Convert.ToInt32(value);
                }

                return 12;
            }
        }

        public static double VirtualRateThresholdForWinning
        {
            get
            {
                string value = ConfigurationManager.AppSettings["VirtualRateThresholdForWinning"];

                if (!string.IsNullOrEmpty(value))
                {
                    return Convert.ToDouble(value);
                }

                return 1.0;
            }
        }

        public static double NewVirtualMultiplierForSuperProPunter
        {
            get
            {
                string count = ConfigurationManager.AppSettings["NewVirtualMultiplierForSuperProPunter"];

                if (!string.IsNullOrEmpty(count))
                {
                    return Convert.ToDouble(count);
                }

                return 1;
            }
        }

        public static double NewVirtualMultiplierForWinning
        {
            get
            {
                string count = ConfigurationManager.AppSettings["NewVirtualMultiplierForWinning"];

                if (!string.IsNullOrEmpty(count))
                {
                    return Convert.ToDouble(count);
                }

                return 1;
            }
        }


    }
}