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
            Group = jobModel.Group;
            RepeatInterval = triggerModel.RepeatInterval;
            CronExpression = triggerModel.CronExpression;
            APIUrl = "";
            JobDescription = jobModel.Description;            
            IsActive = jobModel.IsActive;
        }

        public string JobName { get; set; }

        public Type JobType { get; }

        public string Group { get; set; }        

        public string APIUrl { get; set; }

        public string JobDescription { get; set; }        

        public bool IsActive { get; set; }

        public string TriggerName { get; set; }

        public int RepeatInterval { get; set; }

        public string CronExpression { get; }

    }
}
