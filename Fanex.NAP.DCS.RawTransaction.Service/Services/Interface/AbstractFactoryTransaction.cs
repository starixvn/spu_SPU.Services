namespace Fanex.NAP.DCS.RawTransaction.Service.Services.Interface
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using System.Threading;
    using Fanex.NAP.DCS.Model;

    public abstract class AbstractFactoryTransaction
    {
        public void HandleTransaction(List<RawTransactionModel> transactions, List<Task> tasks, CancellationTokenSource tokenSource)
        {
            if (transactions.Count >= GetBatchThreshold())
            {
                InsertTransaction(transactions);
                transactions.Clear();
            }
            else
            {
                DelayTask(transactions, tasks, tokenSource);
            }
        }
        protected abstract void InsertTransaction(List<RawTransactionModel> transactions);
        protected abstract void DelayTask(List<RawTransactionModel> transactions, List<Task> tasks, CancellationTokenSource tokenSource);
        protected abstract int GetBatchThreshold();
        protected abstract bool IsTransOfDBExtra();
    }
}
