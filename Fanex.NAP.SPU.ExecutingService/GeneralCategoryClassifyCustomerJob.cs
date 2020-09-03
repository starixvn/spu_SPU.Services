namespace Fanex.NAP.SPU.ExecutingService
{
    using Fanex.Logging;
    using Fanex.Logging.Extensions;
    using Fanex.Logging.Extensions.Database;
    using Fanex.NAP.CTS.Model;
    using Fanex.NAP.CTS.Services;
    using Fanex.NAP.FS.Core;
    using Newtonsoft.Json;
    using Quartz;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    [PersistJobDataAfterExecution]
    [DisallowConcurrentExecution]
    public class GeneralCategoryClassifyCustomerJob : ExecuteJobBase, IJob
    {
        readonly DataCenterService dataService = new DataCenterService();

        public Task Execute(IJobExecutionContext context)
        {
            return Task.Run(() => StartJob(context));

        }

        private void StartJob(IJobExecutionContext context)
        {
            var jobDataMap = context.Trigger.JobDataMap;
            var frequency = jobDataMap.GetInt("Frequency");
            switch (frequency)
            {
                case (int)FrequencyExecution.Realtime:
                    RealtimeClassify(jobDataMap);
                    break;
                case (int)FrequencyExecution.Daily:
                    DailyClassify(jobDataMap);
                    break;
                default:
                    LogService.Error("Frequency Data is not correct!");
                    break;
            }
        }

        private void RealtimeClassify(JobDataMap jobDataMap)
        {
            try
            {
                int batchSize = jobDataMap.GetInt("BatchSize");
                var customerHasChanges = dataService.GetCustomerHasChangeForRunRealtime();

                if (customerHasChanges.Any())
                {
                    var custChunks = customerHasChanges.Chunk(batchSize).ToList();
                    for (int i = 0; i < custChunks.Count; i++)
                    {
                        var custSportJson = JsonConvert.SerializeObject(custChunks[i]);
                        var customers = dataService.GetCustomerForRunRealtime(custSportJson);
                        dataService.ClassifyCustomerRealtime(customers);
                        Thread.Sleep(3000);
                    }

                    dataService.RealtimeUpdateLastScanTime();
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

        private void DailyClassify(JobDataMap jobDataMap)
        {
            IEnumerable<CustomerClassificationModel> customers = null;
            int batchSize = jobDataMap.GetInt("BatchSize");
            int parentCateID = jobDataMap.GetInt("ParentCategoryID");
            int countTotalCusts = 0;

            do
            {
                try
                {
                    customers = dataService.GetCustomerForRunDaily(1, batchSize, parentCateID);

                    if (customers.Any())
                    {
                        var custGroupByBatch = customers.GroupBy(c => c.BatchID);
                        foreach (var custGroup in custGroupByBatch)
                        {
                            dataService.ClassifyCustomerGeneralCategoryDaily(custGroup);
                            string jsonData = JsonConvert.SerializeObject(custGroup.Select(a => new { a.CustId, a.SportGroupId }));
                            dataService.ClassifyCustomerDailyDone(jsonData, parentCateID);
                            countTotalCusts += custGroup.Count();
                        }
                    }
                }
                catch (Exception ex)
                {
                    var logMessage = new LogMessage(ex.Message)
                        .AddSection("Data Info", DatabaseInfoGetters.GetDbExceptionInfo(ex))
                        .AddSection("Exception Info", CommonInfoGetters.GetExceptionInfo(ex)).ToString();

                    LogService.Error(logMessage);
                }

            } while (customers == null || customers.Any());

            LogService.Info("Classify Normal customer daily: " + countTotalCusts + " Date: " + DateTime.Now.ToString());
        }
    }
}
