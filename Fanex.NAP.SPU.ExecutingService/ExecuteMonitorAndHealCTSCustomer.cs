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
                LogService.Info("Start Healing Customer job");
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
            bool healingDone = false;
            List<CustomerMainDbModel> mainDBCusts;

            if (jobDataMap != null)
            {
                var rootPath = jobDataMap.GetString(AppSetting.ClusterKey);

                var isActive = jobDataMap.GetBoolean("Active");
                var packageSize = jobDataMap.GetInt("PackageSize");
                int maxCustId = AppSetting.MaxCustIdForHealing;

                if (isActive && !string.IsNullOrWhiteSpace(rootPath))
                {
                    var apiUrls = rootPath.Split(';');

                    while (!healingDone)
                    {
                        mainDBCusts = dataCenterService.GetCustomerFromMainDB(packageSize, apiUrls.Count(), maxCustId);

                        if (!mainDBCusts.Any())
                        {
                            maxCustId = 0;
                            while (true)
                            {
                                mainDBCusts = dataCenterService.GetCustSubFromMainDB(packageSize, apiUrls.Count(), maxCustId);

                                if (!mainDBCusts.Any())
                                {
                                    healingDone = true;
                                    break;
                                }

                                maxCustId = mainDBCusts.Max(c => c.CustSubId);
                                await SendRequest(mainDBCusts, apiUrls, jobDataMap);
                            }
                        }
                        else
                        {
                            maxCustId = mainDBCusts.Max(c => c.CustId);
                            await SendRequest(mainDBCusts, apiUrls, jobDataMap);
                        }
                    }
                }
            }
            else
            {
                LogService.Error("Job Data cannot be null");
            }
        }

        private async Task SendRequest(List<CustomerMainDbModel> listCust, string[] urlList, JobDataMap mappingData)
        {
            var groupTrans = listCust.GroupBy(x => x.BatchId);
            var tasks = new List<Task>();
            var index = 0;

            foreach (var trans in groupTrans)
            {
                var task = Task.Run(() =>
                {
                    var apiUrl = urlList[index >= urlList.Length ? 0 : index];

                    return Executing(trans.ToList(), apiUrl, mappingData.GetString("Action"));
                });

                await Task.Delay(10);

                tasks.Add(task);

                index++;
            }

            await Task.WhenAll(tasks.ToArray());
        }
    }
}
