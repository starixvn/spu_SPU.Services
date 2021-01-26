namespace Fanex.NAP.SPU.Main.Features.JobSchedulers.ViewModels.Job
{
    using Fanex.NAP.SPU.Main.Features.JobSchedulers.Models;
    using System;

    public class JobScheduleViewModel
    {
        public JobScheduleViewModel(string groupJob, JobModel jobModel, TriggerModel triggerModel)
        {
            Group = groupJob;

            JobName = jobModel.JobName;
            JobType = Type.GetType(jobModel.JobType);            
            APIUrl = "";           

            TriggerType = triggerModel.TriggerType;
            TriggerName = triggerModel.TriggerName;
            RepeatForever = triggerModel.RepeatForever;
            CronExpression = triggerModel.CronExpression;
        }

        public string Group { get; set; }

        public string JobName { get; set; }

        public Type JobType { get; }       

        public string APIUrl { get; set; }     

        public string TriggerType { get; set; }

        public string TriggerName { get; set; }

        public bool RepeatForever { get; set; }

        public string CronExpression { get; }

        public bool IsNew { get; set; }

        public CronTriggerViewModel Cron { get; set; } = new CronTriggerViewModel();

        public bool Validate()
        {
            if (string.IsNullOrEmpty(JobName))
            {
                return false;
            }

            return Cron.Validate(this);
        }

    }
}
