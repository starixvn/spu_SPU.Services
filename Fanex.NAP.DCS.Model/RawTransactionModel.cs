using System;
using System.Numerics;

namespace Fanex.NAP.DCS.Model
{
    public class RawTransactionModel
    {
        public string DeviceCode { get; set; }

        public string InvalidDevice { get; set; }

        public int TransStatus { get; set; }

        public string LoginName { get; set; }

        public string SubscriberName { get; set; }

        public DateTime TransTime { get; set; }

        public string IP { get; set; }

        public BigInteger IPId { get; set; }

        public int PluginID { get; set; }

        public string URL { get; set; }

        public string ActionResult { get; set; }

        public string Action { get; set; }

        public string UserAgent { get; set; }

        public string FingerprintCode { get; set; }

        public string FingerprintMoreInfo { get; set; }
    }
}
