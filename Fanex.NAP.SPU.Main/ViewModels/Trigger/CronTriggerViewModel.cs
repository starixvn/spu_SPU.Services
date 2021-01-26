namespace Fanex.NAP.SPU.Main.ViewModels
{
    using Fanex.NAP.SPU.Main.ViewModels.Job;
    using Quartz;
    using System;
    using System.Collections.Generic;

    public class CronTriggerViewModel : IHasValidation
    {
        public void Validate(ICollection<ValidationError> errors)
        {
            throw new NotImplementedException();
        }

        public void Apply(TriggerBuilder builder, JobScheduleViewModel model)
        {
            builder.WithCronSchedule(model.CronExpression);
        }
    }
}
