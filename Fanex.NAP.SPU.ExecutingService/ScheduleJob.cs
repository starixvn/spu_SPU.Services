using Fanex.NAP.SPU.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fanex.NAP.SPU.ExecutingService
{
    public abstract class ScheduleJob
    {
        public abstract void CreateScheduleJob(JobDataSevices jobData);

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
