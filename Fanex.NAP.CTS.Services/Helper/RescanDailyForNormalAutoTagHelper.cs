namespace Fanex.NAP.CTS.Services.Helper
{
    using Fanex.Logging;
    using Fanex.Logging.Extensions;
    using Fanex.Logging.Extensions.Database;
    using Fanex.NAP.CTS.Model;
    using Fanex.NAP.FS.Core;
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Linq;

    public static class RescanDailyForNormalAutoTagHelper
    {
        public static void RescanNormalDaily(List<TypeCategoryCustomerModel> customers)
        {
            if (customers == null || !customers.Any())
            {
                return;
            }

            var batchSize = Convert.ToInt32(ConfigurationManager.AppSettings["Batch-Size"]);

            var custIdsChunk = customers.Chunk(batchSize).ToList();

            try
            {
                foreach (var custIds in custIdsChunk)
                {
                    var lastestClassifications = WrapperDataCenterService.Instance.GetLatestClassification(custIds);

                    var custIdsToBeRescaned = string.Join(",", lastestClassifications.Select(cust => cust.CustId));

                    WrapperDataCenterService.Instance.ClassifyCustomerGeneralCategoryDaily(custIdsToBeRescaned);
                }
            }
            catch (Exception ex)
            {
                var logMessage = new LogMessage(ex.Message)
                    .AddSection("Data Info", DatabaseInfoGetters.GetDbExceptionInfo(ex))
                    .AddSection("Exception Info", CommonInfoGetters.GetExceptionInfo(ex)).ToString();

                LogService.Error(logMessage);
            }
        }
    }
}
