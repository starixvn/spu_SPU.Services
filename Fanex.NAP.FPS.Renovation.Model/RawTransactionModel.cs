using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Fanex.NAP.FPS.Renovation.Model
{
    public class RawTransactionModel
    {
        public string DeviceCode { get; set; }

        public string InvalidDevice { get; set; }

        public int TransStatus { get; set; }

        public string LoginName { get; set; }

        public string SubscriberName { get; set; }

        public DateTime TransDate { get; set; }

        public string IP { get; set; }

        public int PluginID { get; set; }

        public string URL { get; set; }

        public string ActionResult { get; set; }

        public string UserAgent { get; set; }

        public string FingerprintCode { get; set; }

        public string FingerprintMoreInfo { get; set; }
    }
}
