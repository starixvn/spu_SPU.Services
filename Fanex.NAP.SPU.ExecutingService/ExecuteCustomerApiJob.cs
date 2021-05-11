namespace Fanex.NAP.SPU.ExecutingService
{
    using Fanex.Logging;
    using Fanex.Logging.Extensions;
    using Fanex.Logging.Extensions.Database;
    using Fanex.NAP.DCS.Services.Implement;
    using Fanex.NAP.DCS.Services.Interface;
    using Fanex.NAP.FS.Core;
    using Fanex.NAP.SPU.Model;
    using Quartz;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    [PersistJobDataAfterExecution]
    [DisallowConcurrentExecution]
    public class ExecuteCustomerApiJob : ExecuteJobBase, IJob
    {
        private readonly IDataCenterService dataCenterService;

        public ExecuteCustomerApiJob()
        {
            dataCenterService = new DataCenterService();
        }

        public async Task Execute(IJobExecutionContext context)
        {
            try
            {
                if (!this.CheckUM())
                {
                    await ExecuteAPI(context);
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

        private async Task ExecuteAPI(IJobExecutionContext context)
        {
            var jobDataMap = context.JobDetail.JobDataMap;

            if (jobDataMap != null)
            {
                var rootPath = jobDataMap.GetString(AppSetting.ClusterKey);
                var isActive = jobDataMap.GetBoolean("Active");
                var packageSize = jobDataMap.GetInt("PackageSize");

                if (isActive && !string.IsNullOrWhiteSpace(rootPath))
                {
                    var lastScanned = dataCenterService.GetLastInsertCust();

                    var customers = dataCenterService.GetCustomerFromMainDB(packageSize, lastScanned.LastCustID);

                    var custSubs = dataCenterService.GetCustSubFromMainDB(packageSize, lastScanned.LastCustSubID).ToList();

                    var custsToCTS = PrepareData(customers, custSubs);

                    await SendRequest(custsToCTS, rootPath.Split(';'), jobDataMap);
                }
            }
            else
            {
                LogService.Error("Job Data cannot be null");
            }
        }

        private async Task SendRequest(UpdateCustomerModel listCust, string[] urlList, JobDataMap mappingData)
        {
            var tasks = new List<Task>();
            var custs = new List<UpdateCustomerModel>() { listCust };

            var task = Task.Run(() =>
                {
                    var apiUrl = urlList[0];

                    return Executing(custs, apiUrl, mappingData.GetString("Action"));
                });

            await Task.Delay(10);

            tasks.Add(task);

            await Task.WhenAll(tasks.ToArray());
        }

        private UpdateCustomerModel PrepareData(UpdateCustomerModel customers, List<CustomerMainDBModel> custSubs)
        {
            var custsToCTS = new UpdateCustomerModel()
            {
                CustomerInfos = new List<CustomerMainDBModel>(),
                LastInsertCustID = null
            };

            custsToCTS.CustomerInfos.AddRange(customers?.CustomerInfos);
            custsToCTS.CustomerInfos.AddRange(custSubs);
            custsToCTS.LastInsertCustID = customers?.LastInsertCustID;

            return custsToCTS;
        }
    }
}
