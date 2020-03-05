using System;

namespace Fanex.NAP.DCS.Model
{
    public class DevicePackage
    {
        public long TransId { get; set; }

        public long AccountId { get; set; }

        public long SubscriberId { get; set; }

        public string UserAgentKey { get; set; }

        public string DeviceCode { get; set; }

        public string FingerprintCode { get; set; }

        public string FingerprintMoreInfo { get; set; }

        public DateTime TransTime { get; set; }

        public DateTime CreatedDate { get; set; }

        public int BatchId { get; set; }
    }
}
