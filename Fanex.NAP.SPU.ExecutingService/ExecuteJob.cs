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
    using System.IO;
    using System.Text;
    using System.Threading.Tasks;

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
            if (context != null)
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
                if (!this.CheckUM())
                {
                    var jobData = GetJobDataMapping(context);

                    if (jobData != null)
                    {
                        
                        if(jobData.Action != null)
                        {

                        }
                        else
                        {
                            return ExecuteConsole(context, jobData);
                        }                        
                    }                    
                }
            }
            catch (Exception ex)
            {
                return Task.FromException(new JobExecutionException(ex));
            }

            return Task.CompletedTask;
        }

        private Task ExecuteConsole(IJobExecutionContext context, JobData jobData)
        {
            var outputError = new StringBuilder();

            var jobDataMap = context.JobDetail.JobDataMap;

            var rootPath = jobDataMap.GetString(AppSetting.ClusterKey);

            var path = Path.Combine(rootPath, jobData.FileName);

            if (!CheckActive(context, jobData) || !CheckFileExist(context, path))
            {
                return Task.CompletedTask;
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

            return Task.CompletedTask;
        }

        private bool CheckActive(IJobExecutionContext context, JobData jobData)
        {
            if (!jobData.Active)
            {
                var msg = $"{context.JobDetail.Key} - Execute: not activate";
                LogService.Error(msg);

                if (!context.JobDetail.JobDataMap.Contains(FlagNotifyInActive))
                {
                    if (AppSetting.Environment == 3)
                    {
                        new LetstalkService().Error(msg);
                    }

                    context.JobDetail.JobDataMap.Put(FlagNotifyInActive, true);
                }
            }

            return jobData.Active;
        }

        private bool CheckFileExist(IJobExecutionContext context, string path)
        {
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