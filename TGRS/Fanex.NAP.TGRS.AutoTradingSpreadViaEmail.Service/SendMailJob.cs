namespace Fanex.NAP.TGRS.AutoTradingSpreadViaEmail.Service
{
    using Fanex.MailDelivery.Client.Jobs;
    using Fanex.MailDelivery.Core.Models;
    using Quartz;
    using System;
    using System.IO;

    public class SendMailJob : JobBase
    {
        protected override JobMessage Run(Schedule schedule, IJobExecutionContext jobContext)
        {
            var endpoint = "AutoTrading/GetAutoTradingSpread8LiveExcel";
            var toDate = DateTime.Now.AddDays(-(int)DateTime.Now.DayOfWeek).ToString("yyyy.MMM.dd");
            var saveFileName = "Live Performance Report - " + toDate + ".xlsx";
            var directorySaveFile = AppDomain.CurrentDomain.BaseDirectory + @"Attachment\";

            if (!Directory.Exists(directorySaveFile))
            {
                Directory.CreateDirectory(directorySaveFile);
            }

            var service = new Service(directorySaveFile)
                    .MoveFileToArcFolder()
                    .GenerateAutoTradingSpread8LiveToFile(endpoint, saveFileName);

            var mail = service.GenerateMail(saveFileName);

            return mail == null
                ? new JobMessage
                {
                    Mail = null,
                    Schedule = schedule,
                    JobStatus = JobStatus.JobFailed
                }
                : new JobMessage
                {
                    Mail = mail,
                    Schedule = schedule,
                    JobStatus = JobStatus.Success
                };
        }
    }
}
