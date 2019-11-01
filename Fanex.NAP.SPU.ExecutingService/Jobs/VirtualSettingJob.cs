namespace Fanex.NAP.SPU.ExecutingService.Jobs
{
    using Fanex.NAP.FS.Core;
    using Fanex.NAP.SPU.Model;
    using Quartz;
    using System.Collections.Generic;
    using System.Linq;

    public class VirtualSettingJob : ScheduleJob
    {
        private const string groupName = "VirtualSetting";

        private List<IScheduler> schedulers;

        public override void CreateScheduleJob<T>(JobDataSevices jobData)
        {
            schedulers = new List<IScheduler>();

            foreach (var job in jobData.Jobs)
            {
                var jobBase = new JobBase();
                var jobName = string.Format("{0}Job", job.Name);
                var triggerName = string.Format("{0}Trigger", job.Name);

                jobBase.CreateJob<T>(jobName, groupName, job)
                    .CreateTriggerWithCronSchedule(triggerName, groupName, AppSetting.VSJobCronEveryDay)
                    .CreateSchedule()
                    .CreateListenerJob()
                    .Run();

                schedulers.Add(jobBase.Schedule);
            }
        }

        public override void Shutdown()
        {
            if (schedulers.Any())
            {
                schedulers.ForEach(x => x.Shutdown(false));
            }
        }
    }
}