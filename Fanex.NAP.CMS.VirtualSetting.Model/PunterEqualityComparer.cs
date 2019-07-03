namespace Fanex.NAP.CMS.VirtualSetting.Model
{
    using System;
    using System.Collections.Generic;

    public class PunterEqualityComparer : IEqualityComparer<Punter>
    {
        public bool Equals(Punter x, Punter y)
        {
            if (x == null && y == null)
            {
                return true;
            }
            else if (x == null | y == null)
            {
                return false;
            }
            else if (x.CustID == y.CustID)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public int GetHashCode(Punter obj)
        {
            return obj.CustID.GetHashCode();
        }
    }
}
