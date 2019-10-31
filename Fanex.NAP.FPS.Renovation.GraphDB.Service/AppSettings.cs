  
  
 


namespace Fanex.NAP.FPS.Renovation.GraphDB.Service
{
    using System.Configuration;

    public static class AppSettings
    {
                public static string DB_MiddleService => ConfigurationManager.AppSettings["DB.MiddleService"];
                public static string IsTurnOnQueueHandling => ConfigurationManager.AppSettings["IsTurnOnQueueHandling"];
                public static string RabbitMQ_VirtualHost => ConfigurationManager.AppSettings["RabbitMQ-VirtualHost"];
                public static string RabbitMQ_Host => ConfigurationManager.AppSettings["RabbitMQ-Host"];
                public static string RabbitMQ_PrefetchCount => ConfigurationManager.AppSettings["RabbitMQ-PrefetchCount"];
                public static string RabbitMQ_Username => ConfigurationManager.AppSettings["RabbitMQ-Username"];
                public static string RabbitMQ_Password => ConfigurationManager.AppSettings["RabbitMQ-Password"];
                public static string RabbitMQ_Port => ConfigurationManager.AppSettings["RabbitMQ-Port"];
                public static string RabbitMQ_Queue => ConfigurationManager.AppSettings["RabbitMQ-Queue"];
                public static string RabbitMQ_Exchange => ConfigurationManager.AppSettings["RabbitMQ-Exchange"];
                public static string RabbitMQ_RoutingKey => ConfigurationManager.AppSettings["RabbitMQ-RoutingKey"];
                public static string Log_SentryLogging_Config => ConfigurationManager.AppSettings["Log-SentryLogging-Config"];
                public static string Log_SentryLogging_Category => ConfigurationManager.AppSettings["Log-SentryLogging-Category"];
                public static string AppData_Path => ConfigurationManager.AppSettings["AppData-Path"];
                public static string Trans_Batch => ConfigurationManager.AppSettings["Trans-Batch"];
            }
}

