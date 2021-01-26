namespace Fanex.NAP.SPU.Main.ViewModels.Job
{
    using Fanex.NAP.SPU.Main.Models;
    using System;

    public class JobScheduleViewModel
    {
        public JobScheduleViewModel(JobModel jobModel, TriggerModel triggerModel)
        {
            JobName = jobModel.JobName;
            JobType = Type.GetType("Fanex.NAP.SPU.Main.JobSchedulers.HelloJob");
            Group = jobModel.Group;         
            APIUrl = "";
            JobDescription = jobModel.Description;
            IsActive = jobModel.IsActive;

            TriggerType = triggerModel.TriggerType;
            TriggerName = triggerModel.TriggerName;
            RepeatForever = triggerModel.RepeatForever;
            CronExpression = triggerModel.CronExpression;
        }

        public string JobName { get; set; }

        public Type JobType { get; }

        public string Group { get; set; }

        public string APIUrl { get; set; }

        public string JobDescription { get; set; }        

        public bool IsActive { get; set; }


        public string TriggerType { get; set; }

        public string TriggerName { get; set; }

        public bool RepeatForever { get; set; }

        public string CronExpression { get; }

        public bool IsNew { get; set; }

        public CronTriggerViewModel Cron { get; set; } = new CronTriggerViewModel();

    }
}
