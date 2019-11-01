using Fanex.NAP.SPU.Model;

namespace Fanex.NAP.SPU.ExecutingService
{
    public abstract class ScheduleJob
    {
        public abstract void CreateScheduleJob<T>(JobDataSevices jobData);

        public abstract void Shutdown();

        public JobData GetJobData(int jobId, JobDataSevices servicesMap)
        {
            var jobData = servicesMap.Jobs.Find(c => c.Id == jobId);

            if (jobData == null)
            {
                return new JobData();
            }

            if (!servicesMap.Active)
            {
                jobData.Active = false;
            }

            return jobData;
        }
    }
}
