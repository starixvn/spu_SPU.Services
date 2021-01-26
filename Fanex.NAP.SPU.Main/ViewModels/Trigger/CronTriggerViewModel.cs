namespace Fanex.NAP.SPU.Main.ViewModels
{
    using Quartz;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class CronTriggerViewModel : IHasValidation
    {
        [Required]
        public string Expression { get; set; }
        public string TimeZone { get; set; }

        public void Validate(ICollection<ValidationError> errors)
        {
            throw new NotImplementedException();
        }

        public void Apply(TriggerBuilder builder, TriggerPropertiesViewModel model)
        {
            builder.WithCronSchedule(Expression, x =>
            {
                if (!string.IsNullOrEmpty(TimeZone))
                    x.InTimeZone(TimeZoneInfo.FindSystemTimeZoneById(TimeZone));

                switch (model.MisfireInstruction)
                {
                    case MisfireInstruction.InstructionNotSet:
                        break;
                    case MisfireInstruction.IgnoreMisfirePolicy:
                        x.WithMisfireHandlingInstructionIgnoreMisfires();
                        break;
                    case MisfireInstruction.CronTrigger.DoNothing:
                        x.WithMisfireHandlingInstructionDoNothing();
                        break;
                    case MisfireInstruction.CronTrigger.FireOnceNow:
                        x.WithMisfireHandlingInstructionFireAndProceed();
                        break;
                    default:
                        throw new ArgumentException("Invalid value: " + model.MisfireInstruction, nameof(model.MisfireInstruction));
                }
            });
        }
    }
}
