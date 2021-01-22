namespace Fanex.NAP.SPU.ExecutingService
{
    using Fanex.Logging;
    using Fanex.Logging.Extensions;
    using Fanex.Logging.Extensions.Database;
    using Fanex.NAP.DCS.Services.Implement;
    using Fanex.NAP.DCS.Services.Interface;
    using Fanex.NAP.FS.Core;
    using Fanex.NAP.SPU.Core;
    using Fanex.NAP.SPU.Model;
    using Newtonsoft.Json;
    using Quartz;
    using RestSharp;
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Linq;
    using System.Threading.Tasks;

    [PersistJobDataAfterExecution]
    [DisallowConcurrentExecution]
    public class ExecuteCustomerApiJob : ExecuteJobBase, IJob
    {
        private readonly IDataCenterService dataCenterService;
        private readonly CTS.Services.DataCenterService ctsDataCenterService;

        public ExecuteCustomerApiJob()
        {
            dataCenterService = new DataCenterService();
            ctsDataCenterService = new CTS.Services.DataCenterService();
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

            if(jobDataMap != null)
            {
                var rootPath = jobDataMap.GetString(AppSetting.ClusterKey);

                var isActive = jobDataMap.GetBoolean("Active");
                var packageSize = jobDataMap.GetInt("PackageSize");

                if (isActive && !string.IsNullOrWhiteSpace(rootPath))
                {
                    var listCustInfo = new List<CustomerMainDbModel>();

                    var apiUrls = rootPath.Split(';');

                    var lastScanned = dataCenterService.GetLastInsertCust();

                    listCustInfo.AddRange(dataCenterService.GetCustomerFromMainDB(packageSize, lastScanned.LastCustID));
                    LogService.Info($"CTSCustomer.Service - Get Cust: {JsonConvert.SerializeObject(listCustInfo)}");

                    var custSub = dataCenterService.GetCustSubFromMainDB(packageSize, lastScanned.LastCustSubID);
                    listCustInfo.AddRange(custSub);
                    LogService.Info($"CTSCustomer.Service - Get CustSub: {JsonConvert.SerializeObject(custSub)}");

                    await SendRequest(listCustInfo, apiUrls, jobDataMap);
                }
            }
            else
            {
                LogService.Error("Job Data cannot be null");
            }           
        }

        private async Task SendRequest(List<CustomerMainDbModel> listCust, string[] urlList, JobDataMap mappingData)
        {
            var groupCusts = listCust.GroupBy(x => x.BatchId);
            var tasks = new List<Task>();
            var index = 0;

            foreach (var custs in groupCusts)
            {
                var task = Task.Run(() =>
                {
                    var apiUrl = urlList[index >= urlList.Length ? 0 : index];

                    return Executing(custs.ToList(), apiUrl, mappingData.GetString("Action"));
                });

                await Task.Delay(10);

                tasks.Add(task);

                index++;
            }

            await Task.WhenAll(tasks.ToArray());
        }
    }
}
