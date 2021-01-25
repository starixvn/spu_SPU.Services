namespace Fanex.NAP.SPU.Main.ViewModels
{
    using System.Collections.Generic;

    public interface IHasValidation
    {
        void Validate(ICollection<ValidationError> errors);
    }
}
