namespace Fanex.NAP.TGRS.AutoTradingViaEmail.Service
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
            var endpoint = "AutoTrading/GetAutoTradingEventLeaguesExcel";
            var fromDate = "07-Nov-2019";
            var toDate = DateTime.Now.AddDays(-(int)DateTime.Now.DayOfWeek).ToString("dd-MMM-yyyy");
            var saveFileName = "Model TA Performance Event Details - " + fromDate + " to " + toDate + ".xlsx";
            var directorySaveFile = AppDomain.CurrentDomain.BaseDirectory + @"Attachment\";

            if (!Directory.Exists(directorySaveFile))
            {
                Directory.CreateDirectory(directorySaveFile);
            }
          
            var service = new Service(directorySaveFile)
                    .MoveFileToArcFolder()
                    .GenerateAutoTradingToFile(endpoint, saveFileName);

            return new JobMessage
            {
                Mail = service.GenerateMail(saveFileName),
                Schedule = schedule,
                JobStatus = service.IsSuccessAPIResponse ? JobStatus.Success : JobStatus.JobFailed
            };        
        }
    }
}