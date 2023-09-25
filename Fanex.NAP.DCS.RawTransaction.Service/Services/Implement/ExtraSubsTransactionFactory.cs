namespace Fanex.NAP.DCS.RawTransaction.Service.Services.Implement
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Fanex.NAP.DCS.Model;
    using Fanex.NAP.DCS.RawTransaction.Service.Services.Interface;
    using Fanex.Logging;
    using Fanex.NAP.SPU.Core;
    using System.Threading;

    public class ExtraSubsTransactionFactory : AbstractFactoryTransaction
    {
        private readonly ICapturingDataService _capturingDataService;
        private Task taskExtraSub = null;
        private readonly CancellationTokenSource tokenSourceExtraSub = new CancellationTokenSource();

        public ExtraSubsTransactionFactory()
        {
            _capturingDataService  = new CapturingDataService();
        }
        protected override void InsertTransaction(List<RawTransactionModel> transactions)
        {
            TransformTransactionByIps(transactions);
            _capturingDataService.InsertTransaction(transactions, true);
            transactions.Clear();
        }

        protected override void DelayTask(List<RawTransactionModel> transactions)
        {
            if(taskExtraSub == null)
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
                        LogService.Info("Task of add transaction extra sub list was canceled.");
                    }
                }, tokenSourceExtraSub.Token);

                taskExtraSub = taskDelay;
            }
        }

        private void ProcessDelayedTask(List<RawTransactionModel> transactions)
        {
            if (transactions.Any())
            {
                InsertTransaction(transactions);
            }
            taskExtraSub = null;
        }

        protected override int GetBatchThreshold() => AppSettings.TransOfExtraSubsBatch;
    }
}
