namespace Fanex.NAP.SPU.MonitoringService
{
    using Fanex.Data;
    using Fanex.Data.SqlServer;
    using Fanex.NAP.SPU.MonitoringService.Services;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Fanex.NAP.SPU.Service;
    using Fanex.NAP.FS.Core;
    using ONEbook.UM.Services;
    using ONEbook.UM.Services.Impl;
    using System.Threading;

    class Program
    {
        static void Main(string[] args)
        {
            do
            {
                if (!CheckUM())
                {
                    CheckServices();
                    Thread.Sleep(new TimeSpan(0,30,0));
                }
            } while (true);
        }

        private static void CheckServices()
        {
            var service = new DataAccessService();
            var lestalkService = new LetstalkService();
            var triggers = service.GetAllSimpleTriggers();

            foreach (var trigger in triggers.Where(t => t.PrevFireTime > 0))
            {
                var halfAHour = new TimeSpan(0, 30, 0);
                var intervalInTicks = trigger.RepeatInterval * TimeSpan.TicksPerMillisecond;
                var interval = trigger.RepeatInterval > halfAHour.Milliseconds ? intervalInTicks : intervalInTicks * 3;
                TimeSpan t = TimeSpan.FromMilliseconds(trigger.RepeatInterval);
                string displayRepeatInterval = string.Format("{0:D2}h {1:D2}m {2:D2}s {3:D3}ms",
                                                            t.Hours,
                                                            t.Minutes,
                                                            t.Seconds,
                                                            t.Milliseconds);

                if (trigger.PrevFireTime + interval < DateTime.UtcNow.Ticks)
                {
                    Console.WriteLine(trigger.TriggerName + " - Notify here...!");
                    var msg = $"{trigger.JobName} is FAILED to start. " +
                        $"\n    Last fire time:      {trigger.DisplayLastFireTime} (UTC)" +
                        $"\n    Repeat interval:   {displayRepeatInterval}";

                    //if (AppSetting.Environment == 3)
                    //{
                    //    new LetstalkService().Error("CTS Monitoring system", msg);
                    //}
                    new LetstalkService().Error("CTS Monitoring system", msg);
                }
            }
        }

        private static bool CheckUM()
        {
            IUMControlAdapter maintenanceService;
            maintenanceService = new UMControlAdapter();
            return maintenanceService.GetUnderMaintenanceTime().IsUnderMaintenanceTime;
        }
    }
}
