using Fanex.NAP.SPU.Model;

namespace Fanex.NAP.SPU.ExecutingService.Jobs
{
    public class FPSJob: ScheduleJob
    {
        private const string groupName = "FPSGroup";

        private readonly JobBase _job;

        public FPSJob(JobBase job)
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
                    .CreateTriggerIntervalInSeconds(job.IntervalInSecond)                   
                    .CreateListenerJob()
                    .Run();               
            }
        }
    }
}
