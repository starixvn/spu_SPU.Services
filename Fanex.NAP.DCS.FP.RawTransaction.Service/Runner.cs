namespace Fanex.NAP.DCS.FP.RawTransaction.Service
{
    using EasyNetQ;
    using Fanex.Logging;
    using Fanex.Logging.Extensions;
    using Fanex.NAP.DCS.FP.RawTransaction.Services;
    using Fanex.NAP.SPU.Queue;
    using Fanex.Oneyed.Infrastructure.Models;
    using System;
    using System.Configuration;
    using System.ServiceProcess;

    partial class Runner : ServiceBase
    {
        private EasyNetQConnector _rabbitMQConsume;
        private readonly ICapturingFPService _capturingFPService;

        public Runner()
        {
            InitializeComponent();

            _capturingFPService = new CapturingFPService();
        }

        protected override void OnStart(string[] args)
        {
            // TODO: Add code here to start your service.
            try
            {

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
            // TODO: Add code here to perform any tear-down necessary to stop your service.
            _rabbitMQConsume.Disposed();

            LogService.Info($"Raw trans service is STOPED at {DateTime.Now}");
        }

        private void ConsumeMessageBus()
        {

        }

        private void ConsumerTransactions()
        {
            if (ConfigurationManager.AppSettings["Queue-Consume"] == "1")
            {
                _rabbitMQConsume.AdvanceBus.Consume<MessageQueue>(_rabbitMQConsume.Queue, (message, info) =>
                {
                    try
                    {
                        var messageBody = message.Body;

                        var ignoreSubs = ConfigurationManager.AppSettings["Ignore-Subs"];

                        if (!string.IsNullOrWhiteSpace(ignoreSubs))
                        {
                            var ignoreSubList = ignoreSubs.Split(';');

                            //if (!ignoreSubList.Any(x => x.Equals(messageBody.Subscriber, StringComparison.CurrentCultureIgnoreCase)))
                            //{
                            //    var tran = TransformRawTransaction(messageBody);

                            //    trans.Add(tran);

                            //    if (trans.Count >= AppSettings.TransBatch)
                            //    {
                            //        InsertTransaction(trans);

                            //        trans.Clear();

                            //    }
                            //}

                        }

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
