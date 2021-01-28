namespace Fanex.NAP.SPU.Main.Features.JobSchedulers.ViewModels
{
    using Fanex.NAP.SPU.Main.Features.JobSchedulers.ViewModels.Job;
    using Quartz;
    using System;
    using System.Collections.Generic;

    public class CronTriggerViewModel
    {
        public bool Validate(JobScheduleViewModel model)
        {
            if (string.IsNullOrEmpty(model.TriggerName))
            {
                return false;
            }

            if (string.IsNullOrEmpty(model.CronExpression))
            {
                return false;
            }

            return true;
        }

        public void Apply(TriggerBuilder builder, JobScheduleViewModel model)
        {
            builder.WithCronSchedule(model.CronExpression);
        }
    }
}
