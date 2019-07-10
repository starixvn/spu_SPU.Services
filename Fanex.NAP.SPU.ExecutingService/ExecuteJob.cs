namespace Fanex.NAP.SPU.ExecutingService
{
    using Fanex.Logging;
    using Fanex.NAP.FS.Core;
    using Fanex.NAP.FS.Service;
    using ONEbook.UM.Services;
    using ONEbook.UM.Services.Impl;
    using Quartz;
    using System;
    using System.Diagnostics;
    using System.Threading;
    using System.Threading.Tasks;

    [PersistJobDataAfterExecution]
    [DisallowConcurrentExecution]
    public class ExecuteJob : IJob
    {
        private const string FlagNotExistsFile = "FlagNotExistsFile";

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
                if (this.CheckUM() || !CheckActive(context) || !CheckFileExist(context))
                {
                    return Task.FromResult(false);
                }

                var path = context.JobDetail.JobDataMap.GetString("Path");
                var process = new Process();
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.RedirectStandardError = true;
                process.StartInfo.FileName = path;
                process.Start();

                process.ErrorDataReceived += Process_ErrorDataReceived;
                process.OutputDataReceived += Process_OutputDataReceived;
                process.BeginErrorReadLine();
                process.BeginOutputReadLine();

                var errorMessage = process.StandardError.ReadToEnd();

                process.WaitForExit();

                if (!string.IsNullOrWhiteSpace(errorMessage))
                {
                    return Task.FromException(new JobExecutionException(errorMessage));
                }
            }

            catch (Exception ex)
            {
                return Task.FromException(new JobExecutionException(ex));
            }

            return Task.FromResult(true);
        }
        

        private void Process_ErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (e.Data != null)
            {
                LogService.Error(e.Data);
                new LetstalkService().Error(e.Data);
                Console.Write(e.Data);
            }
        }

        private void Process_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (e.Data != null)
            {                
                LogService.Info(e.Data);
                Console.Write(e.Data);
            }
        }

        private bool CheckActive(IJobExecutionContext context)
        {
            var active = context.JobDetail.JobDataMap.GetBoolean("Active");

            if (!active)
            {
                var msg = $"{context.JobDetail.Key} - Execute: not activate";
                LogService.Error(msg);
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
                   // new LetstalkService().Error(msg);
                    context.JobDetail.JobDataMap.Put(FlagNotExistsFile, true);
                }

                return false;
            }

            return true;
        }
    }
}
