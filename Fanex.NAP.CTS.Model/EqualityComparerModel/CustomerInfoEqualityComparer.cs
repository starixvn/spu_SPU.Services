namespace Fanex.NAP.CTS.Model
{
    using Fanex.NAP.SPU.Model;
    using System.Collections.Generic;

    public class CustomerInfoEqualityComparer : IEqualityComparer<CustomerMainDBModel>
    {
        public bool Equals(CustomerMainDBModel x, CustomerMainDBModel y)
        {
            if (x.CustID == y.CustID
                && x.CustStatusID == y.CustStatusID
                && x.Danger1 == y.Danger1
                && x.Danger2 == y.Danger2
                && x.Danger3 == y.Danger3)
                return true;

            return false;
        }

        public int GetHashCode(CustomerMainDBModel obj)
        {
            return obj.GetHashCode();
        }
    }
}