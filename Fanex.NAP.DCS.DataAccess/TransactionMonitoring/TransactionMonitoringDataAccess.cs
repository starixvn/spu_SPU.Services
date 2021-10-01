namespace Fanex.NAP.DCS.DataAccess.TransactionMonitoring
{
    using Fanex.Data.Repository;
    using Fanex.NAP.DCS.Criteria.TransactionMonitoring;

    public class TransactionMonitoringDataAccess
    {
        public int SumRawTransaction(string transJson)
        {
            var repository = new DynamicRepository();

            var criteria = new SumRawTransactionCriteria
            {
                TransJson = transJson
            };

            return repository.Execute(criteria);
        }
    }
}
