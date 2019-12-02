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
    public class ExecuteAssociationDeviceAPIJob : ExecuteJobBase, IJob
    {
        private readonly IDataCenterService dataCenterService;

        public ExecuteAssociationDeviceAPIJob()
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

            if(jobDataMap != null)
            {
                var rootPath = jobDataMap.GetString(AppSetting.ClusterKey);

                var isActive = jobDataMap.GetBoolean("Active");
                var packageSize = jobDataMap.GetInt("PackageSize");

                if (isActive && !string.IsNullOrWhiteSpace(rootPath))
                {
                    var apiUrls = rootPath.Split(';');
                    var numOfBatch = apiUrls.Count();

                    var listOfAssociation = dataCenterService.GetRawAssociation(packageSize, numOfBatch);

                    if (listOfAssociation.Any())
                    {
                        var groupTrans = listOfAssociation.GroupBy(x => x.BatchId);
                        var tasks = new List<Task>();
                        var index = 0;

                        foreach (var trans in groupTrans)
                        {
                            var task = Task.Run(() =>
                            {
                                var apiUrl = apiUrls[index >= apiUrls.Length ? 0 : index];

                                return Executing(trans.ToList(), apiUrl, jobDataMap.GetString("Action"));
                            });

                            await Task.Delay(10);

                            tasks.Add(task);

                            index++;
                        }

                        await Task.WhenAll(tasks.ToArray());
                    }
                }
            }
            else
            {
                LogService.Error("Job Data cannot be null");
            }           
        }

        private static async Task<IRestResponse> Executing(List<RawAssociationModel> accounts, string apiUrl, string actionPath)
        {
            var client = new RestClient(new Uri(apiUrl));
            var request = new RestRequest(actionPath, Method.POST);

            request.AddHeader("Content-type", "application/json");
            request.AddJsonBody(accounts);

            return await client.ExecuteTaskAsync(request);
        }
    }
}
