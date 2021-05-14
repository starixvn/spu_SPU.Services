namespace Fanex.NAP.SPU.Model
{
    using System.Collections.Generic;

    public class UpdateCustomerModel
    {
        public List<CustomerMainDBModel> CustomerInfos { get; set; }

        public long? LastInsertCustID { get; set; }
    }
}
