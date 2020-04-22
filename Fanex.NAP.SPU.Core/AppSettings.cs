namespace Fanex.NAP.SPU.Core
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
                        return Convert.ToInt16(Model.Environment.Local);
                    case "2":
                        return Convert.ToInt16(Model.Environment.UAT);
                    default:
                        return Convert.ToInt16(Model.Environment.Production);
                }
            }
        }

        public static string EnvironmentName
        {
            get
            {
                switch(Environment)
                {
                    case 1:
                        {
                            return "LOCAL";
                        }
                    case 2:
                        {
                            return "UAT";
                        }
                    default:
                        {
                            return "PRO";
                        }
                }
            }
        }

        public static string RabbitMQ_QueueName => ConfigurationManager.AppSettings["Queue-Name"];

        public static string RabbitMQ_RoutingKey => ConfigurationManager.AppSettings["Queue-RoutingKey"];

        public static string RabbitMQ_Host => ConfigurationManager.AppSettings["Queue-Host"];

        public static string RabbitMQ_Post => ConfigurationManager.AppSettings["Queue-Port"];

        public static string RabbitMQ_VirtualHost => ConfigurationManager.AppSettings["Queue-VirtualHost"];

        public static string RabbitMQ_UserName => ConfigurationManager.AppSettings["Queue-UserName"];

        public static string RabbitMQ_Password => ConfigurationManager.AppSettings["Queue-Password"];

        public static int RabbitMQ_PrefetchCount => Convert.ToInt16(ConfigurationManager.AppSettings["Queue-PrefetchCount"]);

        public static string RabbitMQ_Exchange => ConfigurationManager.AppSettings["Queue-Exchange"];

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

        public static string JobCronEveryDay => ConfigurationManager.AppSettings["JobCronEveryDay"];

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

        public static decimal Margin200
        {
            get
            {
                string count = ConfigurationManager.AppSettings["Margin200"];

                if (!string.IsNullOrEmpty(count))
                {
                    return Convert.ToDecimal(count);
                }

                return 4;
            }
        }

        public static int MailingListId
        {
            get
            {
                string mailingListId = ConfigurationManager.AppSettings["MailingListId"];

                if (!string.IsNullOrEmpty(mailingListId))
                {
                    return Convert.ToInt32(mailingListId);
                }

                return 80;
            }
        }

        public static int MailingIdToSPU
        {
            get
            {
                string mailingListId = ConfigurationManager.AppSettings["MailingIdToSPU"];

                if (!string.IsNullOrEmpty(mailingListId))
                {
                    return Convert.ToInt32(mailingListId);
                }

                return 80;
            }
        }

        public static int IdleTime
        {
            get
            {
                var value = ConfigurationManager.AppSettings["IdleTime"].ToString();

                if (!string.IsNullOrEmpty(value))
                {
                    return Convert.ToInt32(value);
                }

                return 30;
            }
        }

        public static List<int> ExecutedDate => ConfigurationManager.AppSettings["ExecutedDate"].Split(';').Select(x => Convert.ToInt32(x)).ToList();

        public static string AppDataSetting => ConfigurationManager.AppSettings["AppData-Path"];

        public static int Interval => Convert.ToInt32(ConfigurationManager.AppSettings["Interval"]);

        public static int TransBatch => Convert.ToInt32(ConfigurationManager.AppSettings["Trans-Batch"]);

        public static string HostPathForTransaction => ConfigurationManager.AppSettings["SelfHost-Url-Trans"];

        public static string HostPathForDevice => ConfigurationManager.AppSettings["SelfHost-Url-Device"];

        public static string HostPathForAccount => ConfigurationManager.AppSettings["SelfHost-Url-Account"];

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

        public static string TGRSAutoTradingLetstalkTarget => ConfigurationManager.AppSettings["TGRS-AutoTrading-Letstalk-Target"];

        public static string TGRSAutoTradingLetstalkGroup => ConfigurationManager.AppSettings["TGRS-AutoTrading-Letstalk-Group"];        

        public static string TGRSAutoTradingSpecicalDateExecute => ConfigurationManager.AppSettings["TGRS-AutoTrading-SpecicalDateExecute"];

        public static string TGRSAutoTradingAPIBaseAddress => ConfigurationManager.AppSettings["TGRS-AutoTrading-API-BaseAddress"];

    }
}