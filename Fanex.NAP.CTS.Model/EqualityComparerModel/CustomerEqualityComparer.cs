namespace Fanex.NAP.CTS.Model
{
    using Fanex.NAP.SPU.Model;
    using System.Collections.Generic;

    public class CustomerEqualityComparer : IEqualityComparer<CustomerMainDBModel>
    {
        public bool Equals(CustomerMainDBModel x, CustomerMainDBModel y)
        {
            if (x.CustID == y.CustID
                && string.Equals(x.UserName, y.UserName)
                && string.Equals(x.UserName2, y.UserName2)
                && x.SiteID == y.SiteID
                && string.Equals(x.Site, y.Site)
                && x.RoleID == y.RoleID
                && x.CurrencyID == y.CurrencyID
                && string.Equals(x.Currency, y.Currency)
                && x.SRecommend == y.SRecommend
                && x.MRecommend == y.MRecommend
                && x.Recommend == y.Recommend)
                return true;

            return false;
        }

        public int GetHashCode(CustomerMainDBModel obj)
        {
            return obj.GetHashCode();
        }
    }
}