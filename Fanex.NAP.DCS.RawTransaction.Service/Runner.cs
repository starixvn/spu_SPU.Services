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
    using Newtonsoft.Json;
    using Renci.SshNet.Messages;
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Linq;
    using System.ServiceProcess;
    using System.Threading.Tasks;
    using System.Threading;
    using Fanex.NAP.DCS.RawTransaction.Service.Services.Implement;
    using Fanex.NAP.DCS.RawTransaction.Service.Services.Interface;

    public partial class Runner : ServiceBase
    {
        private EasyNetQConnector _rabbitMQConsume;
        private EasyNetQConnector _rabbitMQMonitor;
        private readonly ICapturingDataService _capturingDataService;
        private readonly IRawTransactionService _transactionService;

        private readonly List<RawTransactionModel> transDcsSub = new List<RawTransactionModel>();
        private readonly List<RawTransactionModel> tranOfExtraSubs = new List<RawTransactionModel>();

        private readonly List<Task> taskDcsSubList = new List<Task>();
        private readonly List<Task> taskExtraSubList = new List<Task>();

        private readonly CancellationTokenSource cancellationTokenSourceDcsSub = new CancellationTokenSource();
        private readonly CancellationTokenSource cancellationTokenSourceExtraSub = new CancellationTokenSource();

        public Runner()
        {
            InitializeComponent();

            _capturingDataService = new CapturingDataService();
            _transactionService = new RawTransactionService();
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
                var logMessage = new LogMessage(ex.Message)
                    .AddSection("Exception Info", CommonInfoGetters.GetExceptionInfo(ex)).ToString();

                LogService.Error(logMessage);
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
            var exchangeName = AppSettings.IsConsumeDCSExtra ? AppSettings.RabbitMQ_DCSExtraExchangeName : AppSettings.RabbitMQ_Exchange;
            var queueName = AppSettings.IsConsumeDCSExtra ? AppSettings.RabbitMQ_DCSExtraQueueName : AppSettings.RabbitMQ_QueueName;

            _rabbitMQConsume = RabbitMQConnector();
            _rabbitMQConsume.CreateBus()
                    .DeclareExchange(exchangeName)
                    .DeclareQueue(queueName)
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
            bool isQueueConsume = IsQueueConsume();
            if (isQueueConsume)
            {
                _rabbitMQConsume.AdvanceBus.Consume(_rabbitMQConsume.Queue, (Action<IMessage<MessageQueue>, MessageReceivedInfo>)((message, info) =>
                {
                    try
                    {
                        var messageBody = message.Body;
                        var isTransOfExtraSubs = IsTransactionOfExtraSubs(messageBody);
                        var tran = _transactionService.TransformRawTransaction(messageBody);

                        if (string.IsNullOrEmpty(tran.LoginName))
                        {
                            LogService.Error("Test Exeption: Subscribers " + tran.SubscriberName + " sent account loginName is null. \nTrans info: " + JsonConvert.SerializeObject(tran));
                        }
                        else
                        {
                            if (isTransOfExtraSubs)
                            {
                                HandleAddTransOfExtraSubs(tran);
                            }
                            else
                            {
                                HandleAddTransOfDCSSubs(tran);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        var logMessage = new LogMessage(ex.Message)
                            .AddSection("Exception Info", CommonInfoGetters.GetExceptionInfo(ex)).ToString();
                        LogService.Error(logMessage);
                    }
                }));
            }
        }

        private static bool IsQueueConsume()
        {
            return AppSettings.IsConsumeDCSExtra ?
                        ConfigurationManager.AppSettings["DCS-Extra-Queue-Consume"] == "1" :
                        ConfigurationManager.AppSettings["Queue-Consume"] == "1";
        }

        private bool IsTransactionOfExtraSubs(MessageQueue messageBody)
        {
            var extraSubs = AppSettings.ExtraSubs;
            var extraSubsTest = AppSettings.ExtraSubsTest;
            var ignoreSubs = AppSettings.IgnoreSubs;

            if (AppSettings.IsReadyDbExtra && !string.IsNullOrWhiteSpace(extraSubs))

            {
                if (!string.IsNullOrEmpty(extraSubsTest) && messageBody.Subscriber == extraSubsTest)
            if(!string.IsNullOrWhiteSpace(message?.BotD))          
            {          

                {
                    var random = new Random();
                    var subIndex = random.Next(0, extraSubsTest.Length);
                    messageBody.Subscriber = extraSubs.Split(';')[subIndex];
                }

                return extraSubs.Split(';').Any(x => x.Equals(messageBody.Subscriber, StringComparison.CurrentCultureIgnoreCase));
            }

            return false;
        }
        
        private BotD TransformBotd(string encodedBotD)
        {
            var botD = Fanex.DI.Core.Encoders.Base64.Decode(encodedBotD);

            if(!string.IsNullOrEmpty(botD))
            {
                return JsonConvert.DeserializeObject<BotD>(botD);
            }

            return new BotD();
        }

                FakeIP = message.FakeIP?.Trim(),
        private void HandleAddTransOfExtraSubs(RawTransactionModel tran)
        {
            tranOfExtraSubs.Add(tran);
            _transactionService.TransformTransactionByIps(tranOfExtraSubs);
            var extraSubsFactory = new ExtraSubsTransactionFactory();
            extraSubsFactory.HandleTransaction(tranOfExtraSubs, taskExtraSubList, cancellationTokenSourceExtraSub);
        }

        private void HandleAddTransOfDCSSubs(RawTransactionModel tran)
        {
            transDcsSub.Add(tran);
            if(transDcsSub.Count >= AppSettings.TransBatch){
                PushTransactionToMonitor(transDcsSub);
            }
            var dcsSubsFactory = new DCSSubsTransactionFactory();
            dcsSubsFactory.HandleTransaction(transDcsSub, taskDcsSubList, cancellationTokenSourceDcsSub);
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
    }
}