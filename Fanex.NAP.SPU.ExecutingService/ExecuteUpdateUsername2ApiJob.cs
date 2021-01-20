namespace Fanex.NAP.SPU.ExecutingService
{
    using Fanex.Logging;
    using Fanex.Logging.Extensions;
    using Fanex.Logging.Extensions.Database;
    using Fanex.NAP.CTS.Services;
    using Fanex.NAP.CTS.Services.Interface;
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
    public class ExecuteUpdateUsername2ApiJob : ExecuteJobBase, IJob
    {
        private readonly IDataCenterService dataCenterService;

        public ExecuteUpdateUsername2ApiJob()
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
            List<GetCustIdModel> custsFromCTS;
            List<GetCustIdModel> custFromMainDB;

            if (jobDataMap != null)
            {
                var rootPath = jobDataMap.GetString(AppSetting.ClusterKey);

                var isActive = jobDataMap.GetBoolean("Active");
                var packageSize = jobDataMap.GetInt("PackageSize");
                Int64 preCTSCustId = 0;

                if (isActive && !string.IsNullOrWhiteSpace(rootPath))
                {
                    var apiUrls = rootPath.Split(';');

                    while (true)
                    {
                        custsFromCTS = dataCenterService.GetCustWithNullUserName2(packageSize, preCTSCustId);

                        if (custsFromCTS == null || !custsFromCTS.Any())
                        {
                            break;
                        }

                        custFromMainDB = dataCenterService.GetCustWithHavingUserName2(custsFromCTS);
                        preCTSCustId = custsFromCTS.Max(c => c.CTSCustID);

                        if(custFromMainDB != null & custFromMainDB.Any())
                        {     
                            await SendRequest(custFromMainDB, apiUrls, jobDataMap);
                        }
                    }
                }
            }
            else
            {
                LogService.Error("Job Data cannot be null");
            }
        }

        private async Task SendRequest(List<GetCustIdModel> listCust, string[] urlList, JobDataMap mappingData)
        {
            var tasks = new List<Task>();

            var task = Task.Run(() =>
            {
                var apiUrl = urlList[0];

                return Executing(listCust, apiUrl, mappingData.GetString("Action"));
            });

            await Task.Delay(10);

            tasks.Add(task);

            await Task.WhenAll(tasks.ToArray());
        }

        //private static async Task<IRestResponse> Executing(List<CustomerMainDbModel> customers, string apiUrl, string actionPath)
        //{
        //    var client = new RestClient(new Uri(apiUrl));
        //    var request = new RestRequest(actionPath, Method.POST);

        //    request.AddHeader("Content-type", "application/json");
        //    request.AddJsonBody(customers);           
            

        //    return await client.ExecuteTaskAsync(request);
        //}
    }
}
