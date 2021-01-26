namespace Fanex.NAP.SPU.Main.ViewModels
{
    using Quartz;
    using System;
    using System.Collections.Generic;

    public class SimpleTriggerViewModel : IHasValidation
    {
        public void Validate(ICollection<ValidationError> errors)
        {
            throw new NotImplementedException();
        }

        public void Apply(TriggerBuilder builder, TriggerPropertiesViewModel model)
        {

        }
    }
}
