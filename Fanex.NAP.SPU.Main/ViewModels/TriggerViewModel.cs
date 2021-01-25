namespace Fanex.NAP.SPU.Main.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public class TriggerViewModel : IHasValidation
    {
        public TriggerPropertiesViewModel Trigger { get; set; }

        public void Validate(ICollection<ValidationError> errors) { }
    }
}
