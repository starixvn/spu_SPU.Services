namespace Fanex.NAP.CTS.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Fanex.Logging;
    using Fanex.NAP.CTS.DataAccess;
    using Fanex.NAP.CTS.Model;
    using Fanex.NAP.CTS.Services.Helper;
    using Fanex.NAP.CTS.Services.Interface;
    using Newtonsoft.Json;
    using static Fanex.NAP.SPU.Core.Constants;

    public class PushDataToExternalService : IPushDataToExternalService
    {
        private readonly IDataCenterService _dataCenterService;
        private readonly DataCenterDataAccess _dataCenterDataAccess;

        public PushDataToExternalService()
        {
            _dataCenterService = new DataCenterService();
            _dataCenterDataAccess = new DataCenterDataAccess();
        }

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
                    var result = _dataCenterService.UpdateCustomerGeneralClassificationtoMainDB(customerClassifications);

                    if (result.ErrorCode != 0 && retryTime == 0)
                    {
                        LogService.Error($"CTS - PushDataToExternalService - exception from MainDB {result.ErrorMsg} with listCust: {JsonConvert.SerializeObject(customerClassifications)}");
                        return false;
                    }
                    else if (result.ErrorCode != 0)
                    {
                        retryTime--;
                    }
                    else
                    {
                        retryTime = -1;
                    }
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

        public bool UpdateDangerLevelToMain(IEnumerable<DangerLevelModel> dangerLevels, DANGERLEVEL typeId, int retryTime)
        {
            if (dangerLevels == null || !dangerLevels.Any())
            {
                return true;
            }

            while (retryTime >= 0)
            {
                try
                {
                    var result = _dataCenterDataAccess.UpdateDangerLevelToMainDB(dangerLevels, typeId);

                    if (result.ErrorCode != 0 && retryTime == 0)
                    {
                        LogService.Error($"PushDataToExternalService - UpdateDangerLevelToMain exception {result.ErrorMsg} with Data: {JsonConvert.SerializeObject(dangerLevels)}");
                        return false;
                    }
                    else if (result.ErrorCode != 0)
                    {
                        retryTime--;
                    }
                    else
                    {
                        retryTime = -1;
                    }
                }
                catch (Exception ex)
                {
                    if (retryTime == 0)
                    {
                        LogService.Error($"PushDataToExternalService - UpdateDangerLevelToMain exception {ex.Message} with Data: {JsonConvert.SerializeObject(dangerLevels)}", ex);

                        return false;
                    }

                    retryTime--;
                }
            }

            return true;
        }
    }
}
