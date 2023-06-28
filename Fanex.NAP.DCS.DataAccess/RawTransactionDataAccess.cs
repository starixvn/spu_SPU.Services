namespace Fanex.NAP.DCS.DataAccess
{
    using Fanex.Data.Repository;
    using Fanex.NAP.DCS.Criteria;

    public class RawTransactionDataAccess
    {
        public int InsertTransaction(string jsonTrans, bool isTransOfDBExtra)
        {
            var repository = new DynamicRepository();

            var criteria = new CapturingTransactionCriteria
            {
                RawTrans = jsonTrans,
                IsTransOfDBExtra = isTransOfDBExtra
            };

           return repository.Execute(criteria);
        }
    }
}
