namespace Fanex.NAP.CTS.Model
{
    using Fanex.NAP.SPU.Model;
    using System.Collections.Generic;

    public class CustomerStaticEqualityComparer : IEqualityComparer<CustomerMainDBModel>
    {
        public bool Equals(CustomerMainDBModel x, CustomerMainDBModel y)
        {
            if (x.CustID == y.CustID && x.CustStatusID == y.CustStatusID)
            {
                return true;
            }    

            return false;
        }

        public int GetHashCode(CustomerMainDBModel obj)
        {
            return obj.GetHashCode();
        }
    }
}