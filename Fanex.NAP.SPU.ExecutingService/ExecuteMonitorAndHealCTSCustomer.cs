namespace Fanex.NAP.SPU.ExecutingService
{
    using Fanex.Logging;
    using Fanex.Logging.Extensions;
    using Fanex.Logging.Extensions.Database;
    using Fanex.NAP.DCS.Services.Implement;
    using Fanex.NAP.DCS.Services.Interface;
    using Fanex.NAP.FS.Core;
    using Fanex.NAP.SPU.Model;
    using Newtonsoft.Json;
    using Quartz;
    using RestSharp;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    [PersistJobDataAfterExecution]
    [DisallowConcurrentExecution]
    public class ExecuteMonitorAndHealCTSCustomer : ExecuteJobBase, IJob
    {
        private readonly IDataCenterService dataCenterService;

        public ExecuteMonitorAndHealCTSCustomer()
        {
            dataCenterService = new DataCenterService();
        }

        public async Task Execute(IJobExecutionContext context)
        {
            try
            {
                LogService.Info("ExecuteMonitorAndHealCTSCustomer start");

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
            TranformCustModel mainDBCusts;

            if (jobDataMap != null)
            {
                var rootPath = jobDataMap.GetString(AppSetting.ClusterKey);

                var isActive = jobDataMap.GetBoolean("Active");
                var packageSize = jobDataMap.GetInt("PackageSize");

                if (isActive && !string.IsNullOrWhiteSpace(rootPath))
                {
                    var apiUrls = rootPath.Split(';');

                    var output = dataCenterService.GetLastUpdatedTime();

                    mainDBCusts = dataCenterService.GetUpdatedCustFromCustHistory(output.LastUpdateTime, output.LastUpdateCustID, packageSize);

                    if (mainDBCusts != null)
                    {
                        await SendRequest(mainDBCusts, apiUrls, jobDataMap);
                    }
                }
            }
            else
            {
                LogService.Error("Job Data cannot be null");
            }
        }

        private async Task SendRequest(TranformCustModel listCust, string[] urlList, JobDataMap mappingData)
        {
            var tasks = new List<Task>();

            var custInfo = new List<TranformCustModel>() { listCust };

            var task = Task.Run(() =>
             {
                 var apiUrl = urlList[0];

                 return Executing(custInfo, apiUrl, mappingData.GetString("Action"));
             });

            await Task.Delay(10);

            tasks.Add(task);

            await Task.WhenAll(tasks.ToArray());
        }
    }
}
