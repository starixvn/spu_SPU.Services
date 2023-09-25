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

        public DCSSubsTransactionFactory()
        {
            _capturingDataService = new CapturingDataService();
        }

        protected override void InsertTransaction(List<RawTransactionModel> transactions)
        {
            TransformTransactionByIps(transactions);
            _capturingDataService.InsertTransaction(transactions, false);
            transactions.Clear();
        }

        protected override void DelayTask(List<RawTransactionModel> transactions, List<Task> tasks, CancellationTokenSource tokenSource)
        {
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

        private void ProcessDelayedTask(List<RawTransactionModel> transactions, List<Task> tasks)
        {
            if (transactions.Any())
            {
                InsertTransaction(transactions);
            }
            tasks.Clear();
        }
    }
}
