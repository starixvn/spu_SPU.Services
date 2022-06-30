namespace Fanex.NAP.DCS.DataAccess.FPRawTransaction
{
    using Fanex.Data.Repository;
    using Fanex.NAP.DCS.Criteria.FPRawTransaction;

    public interface IFPRawTransactionDataAccess
    {
        int InsertTransaction(string jsonTrans);
    }

    public class FPRawTransactionDataAccess : IFPRawTransactionDataAccess
    {
        public FPRawTransactionDataAccess()
        {
        }

        public int InsertTransaction(string jsonTrans)
        {
            var repository = new DynamicRepository();

            var criteria = new FPCapturingTransactionCriteria
            {
                RawTrans = jsonTrans
            };

            return repository.Execute(criteria);
        }
    }
}
