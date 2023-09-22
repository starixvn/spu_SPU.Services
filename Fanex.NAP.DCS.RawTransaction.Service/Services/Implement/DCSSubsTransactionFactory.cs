namespace Fanex.NAP.DCS.RawTransaction.Service.Services.Implement
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Fanex.Logging;
    using Fanex.Logging.Extensions.Database;
    using Fanex.Logging.Extensions;
    using Fanex.NAP.DCS.Model;
    using Fanex.NAP.DCS.RawTransaction.Service.Services.Interface;
    using Fanex.NAP.SPU.Core;

    public class DCSSubsTransactionFactory : AbstractFactoryTransaction
    {
        private readonly ICapturingDataService _capturingDataService;

        public DCSSubsTransactionFactory()
        {
            _capturingDataService = new CapturingDataService();
        }

        protected override void InsertTransaction(List<RawTransactionModel> transactions)
        {
            try
            {
                _capturingDataService.InsertTransaction(transactions, IsTransOfDBExtra());
            }
            catch (Exception ex)
            {
                var logMessage = new LogMessage(ex.Message)
                    .AddSection("Data Info", DatabaseInfoGetters.GetDbExceptionInfo(ex))
                   .AddSection("Exception Info", CommonInfoGetters.GetExceptionInfo(ex)).ToString();

                LogService.Error(logMessage);
            }
        }

        protected override void DelayTask(List<RawTransactionModel> transactions, List<Task> tasks, CancellationTokenSource tokenSource)
        {
            if (!tasks.Any())
            {
                LogService.Info("start DelayTaskAddTransactionDcsSub at: " + DateTime.Now);
                var delayTime = AppSettings.Interval * 60 * 1000;
                var taskDelay = Task.Delay((int)delayTime).ContinueWith((task) =>
                {
                    try
                    {
                        ProcessDelayedTask(transactions, tasks);
                    }
                    catch (OperationCanceledException)
                    {
                        LogService.Info("Task of add transaction dcs sub list was canceled.");
                    }
                }, tokenSource.Token);
                tasks.Add(taskDelay);
            }
        }


        protected override int GetBatchThreshold() => AppSettings.TransBatch;

        protected override bool IsTransOfDBExtra() => false;

        private void ProcessDelayedTask(List<RawTransactionModel> transactions, List<Task> tasks)
        {
            if (transactions.Any())
            {
                InsertTransaction(transactions);
            }
            transactions.Clear();
            tasks.Clear();
            LogService.Info("end TriggerTaskAddTransactionDcsSub at: " + DateTime.Now);
        }
    }
}
