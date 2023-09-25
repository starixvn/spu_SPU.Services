namespace Fanex.NAP.DCS.RawTransaction.Service.Services.Interface
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using System.Threading;
    using Fanex.NAP.DCS.Model;
    using Fanex.NAP.DCS.RawTransaction.Service.Services.Implement;
    using Fanex.Logging.Extensions.Database;
    using Fanex.Logging.Extensions;
    using System;
    using Fanex.Logging;

    public abstract class AbstractFactoryTransaction
    {
        private readonly IRawTransactionService _rawTransactionService;

        public AbstractFactoryTransaction()
        {
            _rawTransactionService = new RawTransactionService();
        }
        public void HandleTransaction(List<RawTransactionModel> transactions)
        {
            try
            {
                InsertTransaction(transactions);
            }
            catch (Exception ex)
            {
                var logMessage = new LogMessage(ex.Message)
                    .AddSection("Data Info", DatabaseInfoGetters.GetDbExceptionInfo(ex))
                   .AddSection("Exception Info", CommonInfoGetters.GetExceptionInfo(ex)).ToString();

                LogService.Error(logMessage);
            }
        }
        public void HandleDelayTask(List<RawTransactionModel> transactions, List<Task> tasks, CancellationTokenSource tokenSource)
        {
            DelayTask(transactions, tasks, tokenSource);
        }
        protected abstract void InsertTransaction(List<RawTransactionModel> transactions);
       
        protected abstract void DelayTask(List<RawTransactionModel> transactions, List<Task> tasks, CancellationTokenSource tokenSource);

        protected void TransformTransactionByIps(List<RawTransactionModel> transactions)
        {
            _rawTransactionService.TransformTransactionByIps(transactions);
        }
       
    }
}
