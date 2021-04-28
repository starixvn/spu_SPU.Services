namespace Fanex.NAP.CTS.Model
{
    using Fanex.NAP.SPU.Model;
    using System.Collections.Generic;

    public class CustomerStaticEqualityComparer : IEqualityComparer<CustomerMainDbModel>
    {
        public bool Equals(CustomerMainDbModel x, CustomerMainDbModel y)
        {
            if (x.CustID == y.CustID && x.CustStatusID == y.CustStatusID)
            {
                return true;
            }    

            return false;
        }

        public int GetHashCode(CustomerMainDbModel obj)
        {
            return obj.GetHashCode();
        }
    }
}