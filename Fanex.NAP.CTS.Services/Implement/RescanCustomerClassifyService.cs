namespace Fanex.NAP.CTS.Services
{
    using Fanex.Logging;
    using Fanex.Logging.Extensions;
    using Fanex.Logging.Extensions.Database;
    using Fanex.NAP.CTS.Model;
    using Fanex.NAP.CTS.Services.Interface;
    using Fanex.NAP.FS.Core;
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class RescanCustomerClassificationService : IRescanCustomerClassificationService
    {
        private readonly IPushDataToExternalService _pushDataToExternalService;
        private readonly IDataCenterService _dataCenterService;

        public RescanCustomerClassificationService()
        {
            _pushDataToExternalService = new PushDataToExternalService();
            _dataCenterService = new DataCenterService();
        }        

        public bool RescanCustomerClassification(List<TypeCategoryCustomerModel> customers)
        {
            var isSuccess = true;

            // Check and rescan problem account
            RescanProblemAccount();

            // Check and rescan normal account
            RescanNormalAccount(customers);

            return isSuccess;
        }

        public bool PushCustomerClassificationToExternal(IEnumerable<TypeCategoryCustomerModel> customers)
        {
            var ccLatestList = _dataCenterService.GetLatestClassification(customers);

            var isSuccess = _pushDataToExternalService.PushCustomerClassificationToExternal(ccLatestList);

            return isSuccess;
        }
      
        private bool RescanProblemAccount()
        {
            throw new NotImplementedException();
        }     

        private bool RescanNormalAccount(List<TypeCategoryCustomerModel> customers)
        {
            if (customers == null || !customers.Any())
            {
                return true;
            }

            var batchSize = Convert.ToInt32(ConfigurationManager.AppSettings["Batch-Size"]);

            var custIdsChunk = customers.Chunk(batchSize).ToList();

            try
            {
                foreach (var custIds in custIdsChunk)
                {
                    var lastestClassifications = _dataCenterService.GetLatestClassification(custIds);

                    var custIdsToBeRescaned = string.Join(",", lastestClassifications.Select(cust => cust.CustId));

                    _dataCenterService.ClassifyCustomerGeneralCategoryDaily(custIdsToBeRescaned);
                }
            }
            catch (Exception ex)
            {
                var logMessage = new LogMessage(ex.Message)
                    .AddSection("Data Info", DatabaseInfoGetters.GetDbExceptionInfo(ex))
                    .AddSection("Exception Info", CommonInfoGetters.GetExceptionInfo(ex)).ToString();

                LogService.Error(logMessage);

                return false;
            }

            return true;
        }

      
    }
}
