namespace Fanex.NAP.DCS.RawTransaction.Service
{
    using EasyNetQ;
    using Fanex.Logging;
    using Fanex.Logging.Extensions;
    using Fanex.NAP.DCS.Model;
    using Fanex.NAP.DCS.Model.TransactionMonitoring;
    using Fanex.NAP.SPU.Core;
    using Fanex.NAP.SPU.Queue;
    using Fanex.Oneyed.Infrastructure.Models;
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Linq;
    using System.ServiceProcess;
    using Fanex.NAP.DCS.RawTransaction.Service.Services.Implement;
    using Fanex.NAP.DCS.RawTransaction.Service.Services.Interface;

    public partial class Runner : ServiceBase
    {
        private EasyNetQConnector _rabbitMQConsume;
        private EasyNetQConnector _rabbitMQMonitor;
        private readonly IRawTransactionService _transactionService;
        private readonly DCSSubsTransactionFactory _dCSSubsTransactionFactory;
        private readonly ExtraSubsTransactionFactory _extraSubsTransactionFactory;

        private readonly List<RawTransactionModel> transDcsSub = new List<RawTransactionModel>();
        private readonly List<RawTransactionModel> tranOfExtraSubs = new List<RawTransactionModel>();

        public Runner()
        {
            InitializeComponent();

            _transactionService = new RawTransactionService();
            _dCSSubsTransactionFactory = new DCSSubsTransactionFactory();
            _extraSubsTransactionFactory = new ExtraSubsTransactionFactory();
        }

        protected override void OnStart(string[] args)
        {
            try
            {
                ConsumeMessageBus();
                MonitorMessageBus();
                ConsumerTransactions();

                LogService.Info($"Raw trans service is STARTED at {DateTime.Now}");
            }
            catch (Exception ex)
            {
                LogException(ex);
            }
        }

        protected override void OnStop()
        {
            DisposedRabbitMQConnector();
        }

        protected override void OnShutdown()
        {
            DisposedRabbitMQConnector();
        }

        private void DisposedRabbitMQConnector()
        {
            _rabbitMQConsume.Disposed();
            _rabbitMQMonitor.Disposed();

            LogService.Info($"Raw trans service is STOPED at {DateTime.Now}");
        }

        private void ConsumeMessageBus()
        {
            _rabbitMQConsume = RabbitMQConnector();
            _rabbitMQConsume.CreateBus()
                    .DeclareExchange(AppSettings.RabbitMQ_Exchange)
                    .DeclareQueue(AppSettings.RabbitMQ_QueueName)
                    .DeclareRoutingKey(AppSettings.RabbitMQ_RoutingKey)
                    .Bind();
        }

        private void MonitorMessageBus()
        {
            _rabbitMQMonitor = RabbitMQConnector();
            _rabbitMQMonitor.CreateBus()
                    .DeclareExchange(AppSettings.RabbitMQ_Exchange_Monitor)
                    .DeclareQueue(AppSettings.RabbitMQ_QueueName_Monitor)
                    .DeclareRoutingKey(AppSettings.RabbitMQ_RoutingKey_Monitor)
                    .Bind();
        }

        private EasyNetQConnector RabbitMQConnector()
        {
            var rabbitMQConnector = new EasyNetQConnector(AppSettings.RabbitMQ_Host + ":" + AppSettings.RabbitMQ_Post,
                                      AppSettings.RabbitMQ_VirtualHost,
                                      AppSettings.RabbitMQ_PrefetchCount,
                                      AppSettings.RabbitMQ_UserName,
                                      AppSettings.RabbitMQ_Password);
            return rabbitMQConnector;
        }

        private void ConsumerTransactions()
        {
            if (ConfigurationManager.AppSettings["Queue-Consume"] == "1")
            {
                _rabbitMQConsume.AdvanceBus.Consume(_rabbitMQConsume.Queue, (Action<IMessage<MessageQueue>, MessageReceivedInfo>)((message, info) =>
                {
                    try
                    {
                        ProcessTransaction(message);
                    }
                    catch (Exception ex)
                    {
                        LogException(ex);
                    }
                }));
            }
        }

        private void ProcessTransaction(IMessage<MessageQueue> message)
        {
            var transaction = _transactionService.TransformRawTransaction(message.Body);
            var isTransOfExtraSubs = IsTransactionOfExtraSubs(transaction.SubscriberName);
            if (isTransOfExtraSubs)
            {
                HandleAddTransOfExtraSubs(transaction);
            }
            else
            {
                HandleAddTransOfDCSSubs(transaction);
            }
        }

        private bool IsTransactionOfExtraSubs(string subscriber)
        {
            var extraSubs = AppSettings.ExtraSubs;
            if (AppSettings.IsReadyDbExtra && !string.IsNullOrWhiteSpace(extraSubs))
            {
                return extraSubs.Split(';').Any(x => x.Equals(subscriber, StringComparison.CurrentCultureIgnoreCase));
            }

            return false;
        }

        private void HandleAddTransOfExtraSubs(RawTransactionModel tran)
        {
            tranOfExtraSubs.Add(tran);
            _extraSubsTransactionFactory.HandleTransaction(tranOfExtraSubs);
        }

        private void HandleAddTransOfDCSSubs(RawTransactionModel tran)
        {
            transDcsSub.Add(tran);
            _dCSSubsTransactionFactory.HandleTransaction(transDcsSub);
            if (transDcsSub.Count >= AppSettings.TransBatch)
            {
                PushTransactionToMonitor(transDcsSub);
                transDcsSub.Clear();
            }
        }

        private void PushTransactionToMonitor(List<RawTransactionModel> trans)
        {
            if (ConfigurationManager.AppSettings["PushTrans-To-Monitor"] == "1")
            {
                var groupTrans = trans.Select(x => { x.TransTime = Convert.ToDateTime(x.TransTime.ToString("MM/dd/yyyy")); return x; })
                                    .GroupBy(x => new { x.SubscriberId, x.TransTime })
                                    .Select(x => new TransactionMonitoringModel
                                    {
                                        SubscriberID = x.Key.SubscriberId,
                                        TransDate = x.Key.TransTime,
                                        TransTotal = x.Count()
                                    });

                if (groupTrans != null && groupTrans.Any())
                {
                    groupTrans.ToList().ForEach(x =>
                    {
                        var message = new Message<TransactionMonitoringModel>(x, new MessageProperties() { DeliveryMode = 2, Priority = 9 });

                        _rabbitMQMonitor.AdvanceBus.Publish(_rabbitMQMonitor.Exchange, _rabbitMQMonitor.RoutingKey, true, message);
                    });
                }
            }
        }
        private void LogException(Exception ex)
        {
            var logMessage = new LogMessage(ex.Message)
                .AddSection("Exception Info", CommonInfoGetters.GetExceptionInfo(ex)).ToString();
            LogService.Error(logMessage);
        }
    }
}