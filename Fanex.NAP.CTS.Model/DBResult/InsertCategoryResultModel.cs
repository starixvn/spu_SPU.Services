namespace Fanex.NAP.CTS.Model
{
    using System.Collections.Generic;

    public class InsertCategoryResultModel
    {
        public string ErrorMessage { get; set; }

        public IEnumerable<int> CustIds { get; set; }
    }
}
