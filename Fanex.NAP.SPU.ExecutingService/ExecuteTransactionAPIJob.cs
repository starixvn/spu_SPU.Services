namespace Fanex.NAP.SPU.ExecutingService
{
    using Fanex.Logging;
    using Fanex.Logging.Extensions;
    using Fanex.Logging.Extensions.Database;
    using Fanex.NAP.FPS.Renovation.Core;
    using Fanex.NAP.FPS.Renovation.Model;
    using Fanex.NAP.FPS.Renovation.Services.Implement;
    using Fanex.NAP.FPS.Renovation.Services.Interface;
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
    public class ExecuteTransactionAPIJob : ExecuteJobBase, IJob
    {
        private readonly IDataTransformService dataTransformService;

        public ExecuteTransactionAPIJob()
        {
            dataTransformService = new DataTransformService();
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

            var rootPath = jobDataMap.GetString(AppSetting.ClusterKey);

            var isActive = jobDataMap.GetBoolean("Active");

            if (isActive && !string.IsNullOrWhiteSpace(rootPath))
            {
                var apiUrls = rootPath.Split(';').ToList();

                var stagingTranIds = dataTransformService.GetStagingRawTransaction(0, AppSettings.SizeOfTransaction, apiUrls.Count);

                if (stagingTranIds.Any() && stagingTranIds.Any(x => x.MinTransId != 0 && x.MaxTransId != 0))
                {
                    var stagingTrans = stagingTranIds.Select((v, i) =>
                    {
                        v.APIUrl = i > apiUrls.Count ? apiUrls[0] : apiUrls[i];

                        return v;
                    });

                    var tasks = new List<Task>();

                    foreach (var trans in stagingTrans)
                    {
                        var task = Task.Run(() =>
                        {
                            return Executing(trans, jobDataMap.GetString("Action"));
                        });

                        await Task.Delay(10);

                        tasks.Add(task);
                    }

                    await Task.WhenAll(tasks.ToArray());
                }
            }
        }

        private static async Task<IRestResponse> Executing(StagingRawTransactionModel trans, string actionPath)
        {
            var client = new RestClient(new Uri(trans.APIUrl));
            var request = new RestRequest(actionPath, Method.POST);

            request.AddHeader("Content-type", "application/json");
            request.AddJsonBody(trans);

            return await client.ExecuteTaskAsync(request);
        }
    }
}
