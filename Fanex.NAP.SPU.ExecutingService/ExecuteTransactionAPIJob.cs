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
using System.Text;
using System.Threading.Tasks;

namespace Fanex.NAP.SPU.ExecutingService
{
    public class ExecuteTransactionAPIJob : ExecuteJobBase
    {
        private readonly IDataTransformService dataTransformService;

        public ExecuteTransactionAPIJob()
        {
            dataTransformService = new DataTransformService();
        }
        public override async Task Execute(IJobExecutionContext context)
        {
            try
            {
                if (!this.CheckUM())
                {
                    var jobData = GetJobDataMapping(context);

                    if (jobData != null && jobData.Action == null)
                    {
                        await ExecuteAPI(context, jobData);
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
        }

        private async Task ExecuteAPI(IJobExecutionContext context, JobData jobData)
        {
            var jobDataMap = context.JobDetail.JobDataMap;

            var rootPath = jobDataMap.GetString(AppSetting.ClusterKey);

            if (!string.IsNullOrWhiteSpace(rootPath))
            {
                var apiUrls = rootPath.Split(';').ToList();

                var stagingTranIds = dataTransformService.GetStagingRawTransaction(0, AppSettings.SizeOfTransaction, apiUrls.Count);

                if (stagingTranIds.Any() && stagingTranIds.Any(x => x.MinStagingTransId != 0 && x.MaxStagingTransId != 0))
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
                            return Executing(trans, jobData.Action);
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
