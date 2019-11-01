using Fanex.NAP.SPU.Model;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fanex.NAP.SPU.ExecutingService.Jobs
{
    public class FPSJob: ScheduleJob
    {
        private const string groupName = "FPSGroup";

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
                    .CreateTriggerIntervalInSeconds(triggerName, groupName, job.IntervalInSecond)
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
