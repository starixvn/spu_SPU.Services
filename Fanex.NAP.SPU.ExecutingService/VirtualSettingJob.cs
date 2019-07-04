using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fanex.NAP.SPU.Model;

namespace Fanex.NAP.SPU.ExecutingService
{
   public class VirtualSettingJob: ScheduleJob
    {
        public override void CreateScheduleJob(JobDataSevices jobData)
        {
            foreach (var job in jobData.Jobs)
            {
                var jobBase = new JobBase();
                var jobName = string.Format("{0}Job", job.Name);
                var triggerName = string.Format("{0}Trigger", job.Name);

                jobBase.CreateJob(jobName, "CMS.VirtualSetting", job)
                    .CreateTrigger(triggerName, 60)
                    .CreateSchedule()
                    .CreateListenerJob()
                    .Run();
            }
        }
    }
}
