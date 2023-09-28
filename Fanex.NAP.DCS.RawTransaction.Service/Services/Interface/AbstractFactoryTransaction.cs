namespace Fanex.NAP.DCS.RawTransaction.Service.Services.Interface
{
    using System.Collections.Generic;
    using Fanex.NAP.DCS.Model;
    using Fanex.NAP.DCS.RawTransaction.Service.Services.Implement;
    using Fanex.Logging.Extensions.Database;
    using Fanex.Logging.Extensions;
    using System;
    using Fanex.Logging;
    using System.Threading.Tasks;

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
                if (transactions.Count >= GetBatchThreshold())
                {
                    InsertTransaction(transactions);
                }
                else
                {
                    DelayTask(transactions);
                }
            }
            catch (Exception ex)
            {
                var logMessage = new LogMessage(ex.Message)
                    .AddSection("Data Info", DatabaseInfoGetters.GetDbExceptionInfo(ex))
                   .AddSection("Exception Info", CommonInfoGetters.GetExceptionInfo(ex)).ToString();

                LogService.Error(logMessage);
            }
        }

        protected abstract void InsertTransaction(List<RawTransactionModel> transactions);
       
        protected abstract void DelayTask(List<RawTransactionModel> transactions);
        protected abstract int GetBatchThreshold();

        protected void TransformTransactionByIps(List<RawTransactionModel> transactions)
        {
            _rawTransactionService.TransformTransactionByIps(transactions);
        }
       
    }
}
