namespace Fanex.NAP.SPU.ExecutingService
{
    using Fanex.Logging;
    using Fanex.NAP.FS.Core;
    using Fanex.NAP.SPU.Model;
    using Fanex.NAP.SPU.Service;
    using ONEbook.UM.Services;
    using ONEbook.UM.Services.Impl;
    using Quartz;
    using System;
    using System.Diagnostics;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Linq;
    using System.IO;
    using System.Collections.Generic;

    [PersistJobDataAfterExecution]
    [DisallowConcurrentExecution]
    public class ExecuteJob : IJob
    {
        private const string FlagNotExistsFile = "FlagNotExistsFile";
        private const string FlagNotifyInActive = "FlagNotifyInActive";

        protected IUMControlAdapter maintenanceService;

        public ExecuteJob()
        {
            maintenanceService = new UMControlAdapter();
        }

        private bool CheckUM()
        {
            return maintenanceService.GetUnderMaintenanceTime().IsUnderMaintenanceTime;
        }

        private JobData GetJobDataMapping(IJobExecutionContext context)
        {
            if(context != null)
            {
                var jobDataMap = context.JobDetail.JobDataMap;

                return new JobData
                {
                    Id = jobDataMap.GetInt("Id"),
                    Name = jobDataMap.GetString("Name"),
                    Active = jobDataMap.GetBoolean("Active"),
                    FileName = jobDataMap.GetString("FileName")
                };
            }

            return null;
        }

        public Task Execute(IJobExecutionContext context)
        {
            try
            {
                var jobData = GetJobDataMapping(context);

                if(jobData != null)
                {
                    var outputError = new StringBuilder();

                    var jobDataMap = context.JobDetail.JobDataMap;

                    var rootPath = jobDataMap.GetString(context.Scheduler.SchedulerInstanceId);                        

                    var path = Path.Combine(rootPath, jobData.FileName);

                    if (this.CheckUM() || !CheckActive(context, jobData) || !CheckFileExist(context, path))
                    {
                        return Task.FromResult(false);
                    }

                    var process = new Process();
                    process.StartInfo.UseShellExecute = false;
                    process.StartInfo.RedirectStandardError = true;
                    process.StartInfo.FileName = path;
                    process.Start();

                    process.ErrorDataReceived += (sender, e) =>
                    {
                        if (e.Data != null)
                        {
                            outputError.Append(e.Data);
                            outputError.Append(System.Environment.NewLine);
                        }
                    };
                    process.BeginErrorReadLine();

                    process.WaitForExit();

                    if (!string.IsNullOrEmpty(outputError.ToString()))
                    {
                        LogService.Error(outputError.ToString());

                        if (AppSetting.Environment == 3)
                        {
                            new LetstalkService().Error(outputError.ToString());
                        }
                    }
                }
                else
                {
                    return Task.FromResult(false);
                }                
            }

            catch (Exception ex)
            {
                return Task.FromException(new JobExecutionException(ex));
            }

            return Task.FromResult(true);
        }

        private bool CheckActive(IJobExecutionContext context, JobData jobData)
        {            
            if (!jobData.Active)
            {
                var msg = $"{context.JobDetail.Key} - Execute: not activate";
                LogService.Error(msg);

                if (!context.JobDetail.JobDataMap.Contains(FlagNotifyInActive))
                {
                    context.JobDetail.JobDataMap.Put(FlagNotifyInActive, true);
                }
            }

            return jobData.Active;
        }

        private bool CheckFileExist(IJobExecutionContext context, string path)
        {
            //var path = context.JobDetail.JobDataMap.GetString("Path");

            if (!Ultilities.CheckFileExist(path))
            {
                var msg = $"{context.JobDetail.Key} - Execute: does not exists a file - Path: {path}";
                LogService.Error(msg);

                if (!context.JobDetail.JobDataMap.Contains(FlagNotExistsFile))
                {
                    if (AppSetting.Environment == 3)
                    {
                        new LetstalkService().Error(msg);
                    }

                    context.JobDetail.JobDataMap.Put(FlagNotExistsFile, true);
                }

                return false;
            }

            return true;
        }
    }
}