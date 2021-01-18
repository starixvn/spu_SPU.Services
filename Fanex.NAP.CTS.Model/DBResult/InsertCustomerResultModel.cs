namespace Fanex.NAP.CTS.Model
{
    using Fanex.NAP.SPU.Model;
    using System.Collections.Generic;

    public class InsertCustomerResultModel
    {
        public string ErrorMessage { get; set; }

        public IEnumerable<InsertCustomerModel> CustomerIds { get; set; }
    }
}
