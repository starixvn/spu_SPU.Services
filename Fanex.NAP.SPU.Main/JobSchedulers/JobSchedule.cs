using System;

namespace Fanex.NAP.SPU.Main.JobSchedulers
{
    public class JobSchedule
    {
        public JobSchedule(Type jobType, string cronExpression, string apiUrl)
        {
            JobType = jobType;
            CronExpression = cronExpression;
            APIUrl = apiUrl;
        }

        public Type JobType { get; }

        public string CronExpression { get; }

        public string APIUrl { get; set; }
    }
}
