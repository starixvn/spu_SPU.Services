namespace Fanex.NAP.TGRS.AutoTradingViaEmail.Console
{
    using Fanex.NAP.SPU.Core;
    using System;
    using System.IO;

    static class Program
    {
        static void Main(string[] args)
        {
            var directorySaveFile = AppDomain.CurrentDomain.BaseDirectory + @"Attachment\";
            var endpoint = "AutoTrading/GetAutoTradingEventLeaguesExcel";
            var fromDate = "07-Nov-2019";
            var toDate = DateTime.Now.AddDays(-(int)DateTime.Now.DayOfWeek).ToString("dd-MMM-yyyy");
            var saveFileName = "Model TA Performance Event Details - " + fromDate + " to " + toDate + ".xls";            

            if (!Directory.Exists(directorySaveFile))
            {
                Directory.CreateDirectory(directorySaveFile);
            }
            
            var service = new Service(directorySaveFile);
            service.MoveFileToArcFolder();
            service.GenereateAutoTradingToFile(endpoint, saveFileName);

            if (service.IsSuccessAPIResponse)
            {
                service.SendMailInternal(AppSettings.TGRSMailingListIdToSPU, saveFileName);
            }
        }

    }
}