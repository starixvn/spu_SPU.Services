namespace Fanex.NAP.SPU.ExecutingService.Jobs
{
    using Fanex.NAP.SPU.Model;

    public class VirtualSettingJob : ScheduleJob
    {
        private const string groupName = "VirtualSetting";

        public override void CreateScheduleJob(JobDataSevices jobData)
        {
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
            }
        }
    }
}
