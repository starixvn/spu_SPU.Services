namespace Fanex.NAP.CTS.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Fanex.Logging;
    using Fanex.Logging.Extensions;
    using Fanex.Logging.Extensions.Database;
    using Fanex.NAP.CTS.Model;
    using Fanex.NAP.CTS.Services.Helper;
    using Newtonsoft.Json;

    public class PushDataToExternalService : IPushDataToExternalService
    {
        public bool PushCustomerClassificationToExternal(IEnumerable<LatestClassificationModel> customerClassifications)
        {
            var isSuccess = true;

            PushCustomerClassificationToMain(customerClassifications, 3);

            PushCustomerClassificationToSoon88(customerClassifications);

            return isSuccess;
        }

        private bool PushCustomerClassificationToMain(IEnumerable<LatestClassificationModel> customerClassifications, int retryTime)
        {
            if (customerClassifications == null || !customerClassifications.Any())
            {
                return true;
            }

            while (retryTime >= 0)
            {
                try
                {
                    WrapperDataCenterService.Instance.UpdateCustomerGeneralClassificationtoMainDB(customerClassifications);

                    retryTime = -1;
                }
                catch (Exception ex)
                {
                    if (retryTime == 0)
                    {
                        var jsonDataForLog = JsonConvert.SerializeObject(customerClassifications);

                        var logMessage = new LogMessage(ex.Message)
                       .AddSection("Data Info", DatabaseInfoGetters.GetDbExceptionInfo(ex))
                       .AddSection("Exception Info", CommonInfoGetters.GetExceptionInfo(ex)).ToString();

                        var jobName = "AutoTag Customer Classification";

                        LogService.Error($"{jobName} - Update New Member Classification Customer to MainDB service - Start Exception {logMessage} with listCust: {jsonDataForLog}", ex);

                        return false;
                    }

                    retryTime--;
                }
            }

            return true;
        }

        private void PushCustomerClassificationToSoon88(IEnumerable<LatestClassificationModel> customerClassifications)
        {            
            FeedDataToSoon88Helper.FeedCCToSoon88(customerClassifications);
        }
    }
}
