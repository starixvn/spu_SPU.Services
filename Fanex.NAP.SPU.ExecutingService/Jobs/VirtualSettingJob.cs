namespace Fanex.NAP.SPU.ExecutingService.Jobs
{
    using Fanex.NAP.FS.Core;
    using Fanex.NAP.SPU.Model;

    public class VirtualSettingJob : ScheduleJob
    {
        private const string groupName = "VirtualSetting";

        private readonly JobBase _job;

        public VirtualSettingJob(JobBase job)
        {
            _job = job ?? new JobBase();

            _job.GetSchedule();
        }

        public override void CreateScheduleJob<T>(JobDataSevices jobData)
        {
            foreach (var job in jobData.Jobs)
            {
                var jobName = string.Format("{0}Job", job.Name);
                var triggerName = string.Format("{0}Trigger", job.Name);

                _job.GroupName = groupName;
                _job.TriggerName = triggerName;
                _job.JobName = jobName;

                _job.CreateJob<T>(job)
                     .CreateTriggerWithCronSchedule(AppSetting.VSJobCronEveryDay)
                     .CreateListenerJob()
                     .Run();
            }
        }
    }
}