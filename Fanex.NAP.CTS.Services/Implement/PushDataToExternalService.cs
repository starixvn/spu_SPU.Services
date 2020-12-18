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
        public bool PushCustomerClassificationToMain(IEnumerable<LatestClassificationModel> customerClassifications, int retryTime)
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
                      
                        LogService.Error($"PushDataToExternalService - PushCustomerClassificationToMain exception {ex.Message} with listCust: {jsonDataForLog}", ex);

                        return false;
                    }

                    retryTime--;
                }
            }

            return true;
        }

        public void PushCustomerClassificationToSoon88(IEnumerable<LatestClassificationModel> customerClassifications)
        {            
            FeedDataToSoon88Helper.FeedCCToSoon88(customerClassifications);
        }
    }
}
