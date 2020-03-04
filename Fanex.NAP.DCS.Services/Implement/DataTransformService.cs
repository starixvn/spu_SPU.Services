namespace Fanex.NAP.DCS.Services.Implement
{
    using Fanex.NAP.DCS.Services.Interface;
    using Fanex.NAP.DCS.Model;
    using System.Collections.Generic;

    public class DataTransformService : IDataTransformService
    {
        private readonly DataAccess.StagingRawTransactionDataAccess stagingRawTransaction;
  
        public DataTransformService()
        {
            stagingRawTransaction = new DataAccess.StagingRawTransactionDataAccess();
        }

        public List<StagingRawTransactionModel> GetStagingRawTransaction(int isProcessed, int numberOfTicket, int numberOfBatch)
        {
            var result = stagingRawTransaction.GetStagingRawTrans(isProcessed, numberOfTicket, numberOfBatch);

            System.Console.WriteLine("1.Get Raw Transaction");

            return result;
        }
    }
}
