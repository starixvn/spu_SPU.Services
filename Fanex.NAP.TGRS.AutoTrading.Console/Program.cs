namespace Fanex.NAP.TGRS.AutoTrading.Console
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Threading.Tasks;

    static class Program
    {
        static void Main(string[] args)
        {
            var directorySaveImage = AppDomain.CurrentDomain.BaseDirectory + @"Attachment\";
            if (!Directory.Exists(directorySaveImage))
            {
                Directory.CreateDirectory(directorySaveImage);
            }

            var service = new Service(directorySaveImage);

            string endpointTAMonthly = "AutoTrading/GetAutoTradingTAMonthly";
            string endpointPTAndAfterVRAdjustment = "AutoTrading/GetAutoTradingPTAndAfterVRAdjustment";
            string endpointRT3AndTAComparition = "AutoTrading/GetAutoTradingRT3AndTAComparition";         

            var tasks = new List<Task>();

            tasks.Add(Task.Run(() => service.GenereateAutoTradingToFile(endpointTAMonthly, "AutoTrading_TAMonthly.png")));
            tasks.Add(Task.Run(() => service.GenereateAutoTradingToFile(endpointPTAndAfterVRAdjustment, "AutoTrading_PTAndAfterVRAdjustment.png")));
            tasks.Add(Task.Run(() => service.GenereateAutoTradingToFile(endpointRT3AndTAComparition, "AutoTrading_RT3AndTAComparition.png")));

            Task.WhenAll(tasks.ToArray()).Wait();
           
            service.SendFileMessageViaLestalk();

        }
    }
}