namespace Fanex.NAP.SPU.ExecutingService
{
    using Fanex.Logging;
    using Fanex.Logging.Extensions;
    using Fanex.Logging.Extensions.Database;
    using Fanex.NAP.DCS.Services.Implement;
    using Fanex.NAP.DCS.Services.Interface;
    using Fanex.NAP.FS.Core;
    using Fanex.NAP.SPU.Core;
    using Quartz;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    [PersistJobDataAfterExecution]
    [DisallowConcurrentExecution]
    public class ExecuteUnsuccessAssociationDeviceAPIJob : ExecuteJobBase, IJob
    {
        private readonly IDataCenterService dataCenterService;

        public ExecuteUnsuccessAssociationDeviceAPIJob()
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
                    var listUrl = new List<string>();

                    var listOfAssociation = dataCenterService.GetRawUnsuccessAssociation(packageSize);

                    if (listOfAssociation.Any())
                    {
                        var groupTrans = listOfAssociation.GroupBy(x => x.BatchId);
                        var tasks = new List<Task>();
                        
                        foreach (var trans in groupTrans)
                        {
                            if (!listUrl.Any())
                            {
                                listUrl.AddRange(apiUrls);
                            }

                            var task = Task.Run(() =>
                            {
                                var apiUrl = listUrl.PopAt(0);

                                return Executing(trans.ToList(), apiUrl, jobDataMap.GetString("Action"));
                            });

                            await Task.Delay(10);

                            tasks.Add(task);
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
    }
}
