namespace Fanex.NAP.SPU.Main.JobSchedulers
{
    using Fanex.NAP.SPU.Main.Models;
    using System;

    public class JobSchedule
    {
        public JobSchedule(JobModel jobModel, TriggerModel triggerModel)
        {                      
            JobName = jobModel.JobName;
            JobType = Type.GetType("Fanex.NAP.SPU.Main.JobSchedulers.HelloJob");
            CronExpression = triggerModel.CronExpression;
            APIUrl = "";
        }

        public string JobName { get; set; }

        public Type JobType { get; }

        public string CronExpression { get; }

        public string APIUrl { get; set; }

    }
}
