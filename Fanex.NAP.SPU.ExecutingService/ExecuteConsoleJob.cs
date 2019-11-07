namespace Fanex.NAP.SPU.ExecutingService
{
    using Fanex.Logging;
    using Fanex.NAP.FS.Core;
    using Fanex.NAP.SPU.Service;
    using Quartz;
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Text;
    using System.Threading.Tasks;

    [PersistJobDataAfterExecution]
    [DisallowConcurrentExecution]
    public class ExecuteConsoleJob : ExecuteJobBase, IJob
    {
        private const string FlagNotExistsFile = "FlagNotExistsFile";

        public Task Execute(IJobExecutionContext context)
        {
            try
            {
                if (!this.CheckUM())
                {
                    return ExecuteConsole(context);
                }
            }
            catch (Exception ex)
            {
                return Task.FromException(new JobExecutionException(ex));
            }

            return Task.CompletedTask;
        }

        private Task ExecuteConsole(IJobExecutionContext context)
        {
            var outputError = new StringBuilder();

            var jobDataMap = context.JobDetail.JobDataMap;

            var rootPath = jobDataMap.GetString(AppSetting.ClusterKey);

            var path = Path.Combine(rootPath, jobDataMap.GetString("FileName"));

            var isActive = jobDataMap.GetBoolean("Active");

            if (!isActive || !CheckFileExist(context, path))
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