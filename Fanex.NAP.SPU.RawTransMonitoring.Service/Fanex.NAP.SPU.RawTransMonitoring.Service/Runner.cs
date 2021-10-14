namespace Fanex.NAP.SPU.RawTransMonitoring.Service
{
    using EasyNetQ;
    using Fanex.Logging;
    using Fanex.Logging.Extensions;
    using Fanex.Logging.Extensions.Database;
    using Fanex.NAP.DCS.Model.TransactionMonitoring;
    using Fanex.NAP.SPU.Core;
    using Fanex.NAP.SPU.Queue;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Configuration;
    using System.Linq;
    using System.ServiceProcess;
    using System.Threading;
    using System.Timers;

    public partial class Runner : ServiceBase
    {
        private EasyNetQConnector _rabbitMQConsume;
        private readonly object _lockObject = new object();
        private readonly ITransactionMonitoringService _transactionMonitoringService;
        private readonly List<TransactionMonitoringModel> _transactionList;
        private readonly System.Timers.Timer _timer;
        private DateTime _lastInsertTrans;        

        public Runner()
        {
            InitializeComponent();

            _transactionMonitoringService = new TransactionMonitoringService();
            _lastInsertTrans = DateTime.Now;
            _transactionList = new List<TransactionMonitoringModel>();

            _timer = new System.Timers.Timer();
            _timer.Elapsed += InsertTransactionByTimer;
            _timer.Interval = AppSettings.Interval;
            _timer.Start();
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

        private void InsertTransactionByTimer(object sender, ElapsedEventArgs e)
        {
            if ((DateTime.Now - _lastInsertTrans).Minutes >= 1 && _transactionList.Count > 0)
            {
                InsertTransactionMonitoring(null, true);
            }
        }

        private void ConsumerData()
        {
            if (ConfigurationManager.AppSettings["Queue-Consume"] == "1")
            {
                _rabbitMQConsume.AdvanceBus.Consume<TransactionMonitoringModel>(_rabbitMQConsume.Queue, (message, info) =>
                {
                    try
                    {
                        InsertTransactionMonitoring(message.Body, false);
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

        private void InsertTransactionMonitoring(TransactionMonitoringModel tran, bool isTimer)
        {
            try
            {
                lock(_lockObject)
                {
                    if (tran != null)
                    {
                        _transactionList.Add(tran);
                    }

                    if (_transactionList.Count > 0)
                    {
                        if (_transactionList.Count >= AppSettings.TransBatch || isTimer)
                        {
                            _transactionMonitoringService.SumRawTransaction(_transactionList);
                           
                            _transactionList.Clear();
                        }
                    }

                    _lastInsertTrans = DateTime.Now;
                }                                     
            }
            catch (Exception ex)
            {
                var logMessage = new LogMessage(ex.Message)
                    .AddSection("Data Info", DatabaseInfoGetters.GetDbExceptionInfo(ex))
                   .AddSection("Exception Info", CommonInfoGetters.GetExceptionInfo(ex)).ToString();

                LogService.Error(logMessage);

                _lastInsertTrans = DateTime.Now;
            }
        }
    }
}
