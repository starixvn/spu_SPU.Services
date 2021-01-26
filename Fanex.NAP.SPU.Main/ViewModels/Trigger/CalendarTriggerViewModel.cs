namespace Fanex.NAP.SPU.Main.ViewModels
{
    using Quartz;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public class CalendarTriggerViewModel : IHasValidation
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
