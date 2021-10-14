namespace Fanex.NAP.SPU.RawTransMonitoring.Service
{
    using Fanex.NAP.DCS.DataAccess.TransactionMonitoring;
    using Fanex.NAP.DCS.Model.TransactionMonitoring;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public interface ITransactionMonitoringService 
    {
        int SumRawTransaction(List<TransactionMonitoringModel> rawTrans);
    }

    public class TransactionMonitoringService : ITransactionMonitoringService
    {
        private readonly TransactionMonitoringDataAccess dataAccess;

        public TransactionMonitoringService()
        {
            dataAccess = new TransactionMonitoringDataAccess();
        }

        public int SumRawTransaction(List<TransactionMonitoringModel> rawTrans)
        {
            var model = JsonConvert.SerializeObject(rawTrans);

            return dataAccess.SumRawTransaction(model);
        }

    }
}
