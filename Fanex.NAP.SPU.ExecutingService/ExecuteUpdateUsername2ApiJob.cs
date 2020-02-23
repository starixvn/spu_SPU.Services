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
            bool updateUsername2Done = false;

            if(jobDataMap != null)
            {
                var rootPath = jobDataMap.GetString(AppSetting.ClusterKey);

                var isActive = jobDataMap.GetBoolean("Active");
                var packageSize = jobDataMap.GetInt("PackageSize");
                int maxCTSCustId = 300000;

                if (isActive && !string.IsNullOrWhiteSpace(rootPath))
                {
                    var apiUrls = rootPath.Split(';');

                    while (!updateUsername2Done)
                    {
                        var customers = dataCenterService.GetCustomerForUpdateUsername2(packageSize, false, maxCTSCustId);
                        var custids = string.Join(",", customers.Select(c => c.CustId.ToString()));
                        var mainDBCusts = dataCenterService.GetUsername2FromMainDb(custids, false);

                        if (!customers.Any())
                        {
                            maxCTSCustId = 0;
                            while (true)
                            {
                                customers = dataCenterService.GetCustomerForUpdateUsername2(packageSize, true, maxCTSCustId);
                                custids = string.Join(",", customers.Select(c => c.CustSubId.ToString()));
                                mainDBCusts = dataCenterService.GetUsername2FromMainDb(custids, true);

                                if (!customers.Any())
                                {
                                    updateUsername2Done = true;
                                    break;
                                }

                                maxCTSCustId = customers.Max(c => c.CTSCustId);
                                await SendRequest(mainDBCusts, apiUrls, jobDataMap);
                            }
                        }
                        else
                        {
                            maxCTSCustId = customers.Max(c => c.CTSCustId);
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

        private static async Task<IRestResponse> Executing(List<CustomerMainDbModel> customers, string apiUrl, string actionPath)
        {
            var client = new RestClient(new Uri(apiUrl));
            var request = new RestRequest(actionPath, Method.POST);

            request.AddHeader("Content-type", "application/json");
            request.AddJsonBody(customers);           
            

            return await client.ExecuteTaskAsync(request);
        }
    }
}
