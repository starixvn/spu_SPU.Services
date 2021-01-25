namespace Fanex.NAP.SPU.Main.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public class TriggerPropertiesViewModel : IHasValidation
    {
        public string DateFormat { get; }
        public string TimeFormat { get; }
        public string DateTimeFormat { get => DateFormat + " " + TimeFormat; }

        public void Validate(ICollection<ValidationError> errors)
        {
            throw new NotImplementedException();
        }
    }
}
