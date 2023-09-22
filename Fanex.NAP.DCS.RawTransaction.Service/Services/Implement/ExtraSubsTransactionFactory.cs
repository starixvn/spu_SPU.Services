using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fanex.Logging.Extensions.Database;
using Fanex.Logging.Extensions;
using Fanex.NAP.DCS.Model;
using Fanex.NAP.DCS.RawTransaction.Service.Services.Interface;
using Fanex.Logging;
using Fanex.NAP.SPU.Core;
using Fanex.NAP.DCS.Core;
using System.Threading;

namespace Fanex.NAP.DCS.RawTransaction.Service.Services.Implement
{
    public class ExtraSubsTransactionFactory : AbstractFactoryTransaction
    {
        private readonly ICapturingDataService _capturingDataService;

        public ExtraSubsTransactionFactory()
        {
            _capturingDataService  = new CapturingDataService();
        }
        protected override void InsertTransaction(List<RawTransactionModel> transactions)
        {
            try
            {
                _capturingDataService.InsertTransaction(transactions, true);
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
                LogService.Info("start DelayTaskAddTransactionExtraSubs at: " + DateTime.Now);
                var delayTime = AppSettings.Interval * 60 * 1000;
                var taskDelay = Task.Delay((int)delayTime).ContinueWith((task) =>
                {
                    try
                    {
                        ProcessDelayedTask(transactions, tasks);
                    }
                    catch (OperationCanceledException)
                    {
                        LogService.Info("Task of add transaction extra sub list was canceled.");
                    }
                }, tokenSource.Token);
                tasks.Add(taskDelay);
            }
        }

        protected override int GetBatchThreshold() => AppSettings.TransOfExtraSubsBatch;

        private void ProcessDelayedTask(List<RawTransactionModel> transactions, List<Task> tasks)
        {
            if (transactions.Any())
            {
                InsertTransaction(transactions);
            }
            transactions.Clear();
            tasks.Clear();
            LogService.Info("end TriggerTaskAddTransactionExtraSubs at: " + DateTime.Now);
        }
    }
}
