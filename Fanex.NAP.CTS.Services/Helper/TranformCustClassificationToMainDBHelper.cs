namespace Fanex.NAP.CTS.Services.Helper
{
    using Fanex.Logging;
    using Fanex.Logging.Extensions;
    using Fanex.Logging.Extensions.Database;
    using Fanex.NAP.CTS.Model;
    using Fanex.NAP.FS.Core;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public static class TranformCustClassificationToMainDBHelper
    {
        public static void InsertNewMemberCategoryToMainDB(List<TypeCategoryCustomerModel> newMembers, bool IsCCAutoTag, int retryTime, int batchSize)
        {
            if (newMembers == null || !newMembers.Any())
            {
                return;
            }

            var newMembersChunk = newMembers.Chunk(batchSize).ToList();

            foreach (var listCust in newMembersChunk)
            {
                var custIds = CheckNewMemberClassification(string.Join(",", listCust));

                var lastestClassifications = WrapperDataCenterService.Instance.GetLatestClassification(listCust);

                SyncCCToMainDB(lastestClassifications, IsCCAutoTag, retryTime);

                UpdateAppliedCustomerClassification(lastestClassifications);
            }

        }

        public static bool SyncCCToMainDB(IEnumerable<LastestClassificationModel> lastestClassifications, bool IsCCAutoTag, int retryTime)
        {
            if (lastestClassifications == null || !lastestClassifications.Any())
            {
                return true;
            }

            while (retryTime >= 0)
            {
                try
                {
                    WrapperDataCenterService.Instance.UpdateCustomerGeneralClassificationtoMainDB(lastestClassifications);

                    retryTime = -1;
                }
                catch (Exception ex)
                {
                    if (retryTime == 0)
                    {
                        var jsonDataForLog = JsonConvert.SerializeObject(lastestClassifications);

                        var logMessage = new LogMessage(ex.Message)
                       .AddSection("Data Info", DatabaseInfoGetters.GetDbExceptionInfo(ex))
                       .AddSection("Exception Info", CommonInfoGetters.GetExceptionInfo(ex)).ToString();

                        var jobName = IsCCAutoTag ? "AutoTag Customer Classification" : "Tranform Customer Service";

                        LogService.Error($"{jobName} - Update New Member Classification Customer to MainDB service - Start Exception {logMessage} with listCust: {jsonDataForLog}", ex);
                         
                        return false;
                    }

                    retryTime--;
                }
            }

            return true;
        }

        public static void UpdateAppliedCustomerClassification(IEnumerable<LastestClassificationModel> lastestClassifications)
        {
            if (lastestClassifications == null || !lastestClassifications.Any())
            {
                return;
            }

            var jsonData = JsonConvert.SerializeObject(lastestClassifications);

            var resultAppliedCC = WrapperDataCenterService.Instance.UpdateAppliedCustomerClassification(jsonData);

            if (resultAppliedCC.ErrorCode != 0)
            {
                LogService.Error($" Update Applied Customer Classification fails - Exception {resultAppliedCC.ErrorMsg} with data: {lastestClassifications}");
            }   
        }

        private static IEnumerable<long> CheckNewMemberClassification(string custIds)
        {
            var customerClass = WrapperDataCenterService.Instance.GetCustomerClassByCustId(custIds);

            return customerClass?.Where(c => c.CustomerClass == null).Select(c => c.CustID);
        }
    }
}
