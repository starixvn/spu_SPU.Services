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
    public class ExecuteDeviceAPIJob : ExecuteJobBase, IJob
    {
        private readonly IDataCenterService dataCenterService;

        public ExecuteDeviceAPIJob()
        {
            dataCenterService = new DataCenterService();
        }

        public async Task Execute(IJobExecutionContext context)
        {
            try
            {
                if (!this.CheckUM())
                {
                    var jobData = GetJobDataMapping(context);

                    if (jobData != null)
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
                var apiUrls = rootPath.Split(';');

                var listOfTrans = dataCenterService.GetStagingTransIdForDevice(AppSettings.SizeOfDevice, apiUrls.Count());

                if (listOfTrans.Any())
                {
                    var groupTrans = listOfTrans.GroupBy(x => x.BatchId);
                    var tasks = new List<Task>();
                    var index = 0;

                    foreach (var trans in groupTrans)
                    {
                        var task = Task.Run(() =>
                        {
                            var apiUrl = apiUrls[index >= apiUrls.Length ? 0 : index];

                            return Executing(trans.ToList(), apiUrl, jobData.Action);
                        });

                        await Task.Delay(10);

                        tasks.Add(task);

                        index++;
                    }

                    await Task.WhenAll(tasks.ToArray());
                }
            }
        }

        private static async Task<IRestResponse> Executing(List<DevicePackage> devices, string apiUrl, string actionPath)
        {
            var client = new RestClient(new Uri(apiUrl));
            var request = new RestRequest(actionPath, Method.POST);

            request.AddHeader("Content-type", "application/json");
            request.AddJsonBody(devices);

            return await client.ExecuteTaskAsync(request);
        }
    }
}
