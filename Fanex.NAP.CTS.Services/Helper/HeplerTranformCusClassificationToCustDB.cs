namespace Fanex.NAP.CTS.Services.Helper
{
    using Fanex.Logging;
    using Fanex.Logging.Extensions;
    using Fanex.Logging.Extensions.Database;
    using Fanex.NAP.CTS.Model;
    using Fanex.NAP.FS.Core;
    using Fanex.NAP.SPU.Core;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public static class HeplerTranformCusClassificationToCustDB
    {
        public static void InsertNewMemberCategoryToMainDB(IEnumerable<CustomerCategoryClassificationModel> newMembers, bool IsCCAutoTag, int retryTime, int batchSize)
        {
            var ctsDataCenterService = new DataCenterService();

            if (newMembers == null || !newMembers.Any())
            {
                return;
            }

            var countRetry = 0;

            while (countRetry < retryTime)
            {
                try
                {
                    var listCustClassChunk = newMembers.Chunk(batchSize).ToList();

                    foreach (var listCust in listCustClassChunk)
                    {
                        ctsDataCenterService.UpdateCustomerGeneralClassificationtoMainDB(listCust.Select(c => new CustomerClassModel
                        {
                            CustID = c.CustID,
                            CustomerClass = Constants.NEW_MEMBER_CLASSIFICATION
                        }).ToList());
                    }

                    countRetry = retryTime + 1;
                }
                catch (Exception ex)
                {
                    var logMessage = new LogMessage(ex.Message)
                        .AddSection("Data Info", DatabaseInfoGetters.GetDbExceptionInfo(ex))
                        .AddSection("Exception Info", CommonInfoGetters.GetExceptionInfo(ex)).ToString();

                    var jobName = IsCCAutoTag ? "AutoTag Customer Classification" : "Tranform Customer Service";

                    LogService.Error($"{jobName} - Update New Member Classification Customer to MainDB service - Start Exception {logMessage}");
                    countRetry += 1;
                }
            }
        }
    }
}
