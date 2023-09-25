namespace Fanex.NAP.DCS.RawTransaction.Service.Services.Implement
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Fanex.Logging;
    using Fanex.NAP.DCS.Model;
    using Fanex.NAP.DCS.RawTransaction.Service.Services.Interface;
    using Fanex.NAP.SPU.Core;

    public class DCSSubsTransactionFactory : AbstractFactoryTransaction
    {
        private readonly ICapturingDataService _capturingDataService;
        private Task taskDcsSub = null;
        private readonly CancellationTokenSource tokenSourceDcsSub = new CancellationTokenSource();

        public DCSSubsTransactionFactory()
        {
            _capturingDataService = new CapturingDataService();
        }

        protected override void InsertTransaction(List<RawTransactionModel> transactions)
        {
            TransformTransactionByIps(transactions);
            _capturingDataService.InsertTransaction(transactions, false);
        }

        protected override void DelayTask(List<RawTransactionModel> transactions)
        {
            if(taskDcsSub == null)
            {
                var delayTime = AppSettings.Interval * 60 * 1000;
                var taskDelay = Task.Delay((int)delayTime).ContinueWith((task) =>
                {
                    try
                    {
                        ProcessDelayedTask(transactions);
                    }
                    catch (OperationCanceledException)
                    {
                        LogService.Info("Task of add transaction dcs sub list was canceled.");
                    }
                }, tokenSourceDcsSub.Token);

                taskDcsSub = taskDelay;
            }
        }

        private void ProcessDelayedTask(List<RawTransactionModel> transactions)
        {
            if (transactions.Any())
            {
                InsertTransaction(transactions);
            }
            taskDcsSub = null;
            transactions.Clear();
        }

        protected override int GetBatchThreshold() => AppSettings.TransBatch;
    }
}
