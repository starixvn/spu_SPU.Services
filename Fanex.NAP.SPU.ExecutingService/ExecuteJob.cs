namespace Fanex.NAP.SPU.ExecutingService
{
    using Fanex.Logging;
    using Fanex.NAP.FS.Core;
    using Fanex.NAP.SPU.Service;
    using ONEbook.UM.Services;
    using ONEbook.UM.Services.Impl;
    using Quartz;
    using System;
    using System.Diagnostics;
    using System.Text;
    using System.Threading;
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

        public Task Execute(IJobExecutionContext context)
        {
            try
            {
                var outputError = new StringBuilder();

                if (this.CheckUM() || !CheckActive(context) || !CheckFileExist(context))
                {
                    return Task.FromResult(false);
                }

                var path = context.JobDetail.JobDataMap.GetString("Path");
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
                        outputError.Append(Environment.NewLine);
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

            catch (Exception ex)
            {
                return Task.FromException(new JobExecutionException(ex));
            }

            return Task.FromResult(true);
        }

        private bool CheckActive(IJobExecutionContext context)
        {
            var active = context.JobDetail.JobDataMap.GetBoolean("Active");

            if (!active)
            {
                var msg = $"{context.JobDetail.Key} - Execute: not activate";
                LogService.Error(msg);

                if (!context.JobDetail.JobDataMap.Contains(FlagNotifyInActive))
                {
                    context.JobDetail.JobDataMap.Put(FlagNotifyInActive, true);
                }
            }

            return active;
        }

        private bool CheckFileExist(IJobExecutionContext context)
        {
            var path = context.JobDetail.JobDataMap.GetString("Path");

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