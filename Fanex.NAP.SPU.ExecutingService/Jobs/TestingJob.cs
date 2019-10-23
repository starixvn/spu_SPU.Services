using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fanex.NAP.SPU.Model;
using Quartz;

namespace Fanex.NAP.SPU.ExecutingService.Jobs
{
    public class TestingJob : ScheduleJob
    {
        private const string groupName = "TestingGroup";

        private List<IScheduler> schedulers;

        public override void CreateScheduleJob(JobDataSevices jobData)
        {
             schedulers = new List<IScheduler>();

            foreach (var job in jobData.Jobs)
            {
                var jobBase = new JobBase();
                var jobName = string.Format("{0}Job", job.Name);
                var triggerName = string.Format("{0}Trigger", job.Name);

                jobBase.CreateJob(jobName, groupName, job)
                    .CreateTrigger(triggerName, groupName, 60)
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
