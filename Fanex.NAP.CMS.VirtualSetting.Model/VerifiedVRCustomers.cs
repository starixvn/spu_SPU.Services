namespace Fanex.NAP.CMS.VirtualSetting.Model
{
    using System.Collections.Generic;

    public class VerifiedVRCustomers
    {
        public VerifiedVRCustomers(IEnumerable<int> passedPerf, IEnumerable<int> unpassedPerf)
        {
            PassedPerfCheckCustomers = passedPerf;
            UnpassedPerfCheckCustomers = unpassedPerf;
        }

        public IEnumerable<int> PassedPerfCheckCustomers { get; set; }

        public IEnumerable<int> UnpassedPerfCheckCustomers { get; set; }
    }
}
