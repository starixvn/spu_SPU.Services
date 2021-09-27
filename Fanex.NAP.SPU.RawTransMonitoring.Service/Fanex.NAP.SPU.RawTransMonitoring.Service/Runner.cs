using EasyNetQ;
using Fanex.Logging;
using Fanex.Logging.Extensions;
using Fanex.NAP.DCS.Model;
using Fanex.NAP.SPU.Core;
using Fanex.NAP.SPU.Queue;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace Fanex.NAP.SPU.RawTransMonitoring.Service
{
    public partial class Runner : ServiceBase
    {
        private EasyNetQConnector _rabbitMQConsume;
        public Runner()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            ConsumeMessageBus();
            ConsumerData();
        }

        protected override void OnStop()
        {
            
            _rabbitMQConsume.Disposed();
        }

        private void ConsumeMessageBus()
        {
            _rabbitMQConsume = new EasyNetQConnector(AppSettings.RabbitMQ_Host + ":" + AppSettings.RabbitMQ_Post,
                                      AppSettings.RabbitMQ_VirtualHost,
                                      AppSettings.RabbitMQ_PrefetchCount,
                                      AppSettings.RabbitMQ_UserName,
                                      AppSettings.RabbitMQ_Password);

            _rabbitMQConsume.CreateBus()
                    .DeclareExchange(AppSettings.RabbitMQ_Exchange_Monitor)
                    .DeclareQueue(AppSettings.RabbitMQ_QueueName_Monitor)
                    .DeclareRoutingKey(AppSettings.RabbitMQ_RoutingKey_Monitor)
                    .Bind();
        }

        private void ConsumerData()
        {
            if (ConfigurationManager.AppSettings["Queue-Consume"] == "1")
            {
                var trans = new List<TransactionMonitoringModel>();

                _rabbitMQConsume.AdvanceBus.Consume<TransactionMonitoringModel>(_rabbitMQConsume.Queue, (message, info) =>
                {
                    try
                    {
                        var messageBody = message.Body;

                        trans.Add(messageBody);

                    }
                    catch (Exception ex)
                    {
                        var logMessage = new LogMessage(ex.Message)
                            .AddSection("Exception Info", CommonInfoGetters.GetExceptionInfo(ex)).ToString();

                        LogService.Error(logMessage);
                    }
                });
            }
        }
    }
}
