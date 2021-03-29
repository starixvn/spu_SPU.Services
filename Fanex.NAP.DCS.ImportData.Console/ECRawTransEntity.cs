using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fanex.NAP.DCS.ImportData.Console
{
   public class ECRawTransEntity
    {
        public string LoginName { get; set; }

        public string SubscriberName { get; set; }

        public DateTime TransTime { get; set; }

        public DateTime CreatedDate { get; set; }

        public string DeviceCode { get; set; }

        public string FingerprintCode { get; set; }

        public string FingerprintMoreInfo { get; set; }

        public string UserAgent { get; set; }

        public string Ip { get; set; }

        public decimal IpId { get; set; }

        public long PluginId { get; set; }

        public string Url { get; set; }

        public string Action { get; set; }

        public string ActionResult { get; set; }

        public string InvalidDevice { get; set; }

        public int TransStatus { get; set; }

        public long FpsTransId { get; set; }

        public int Flagged { get; set; }

        public int IsProcessed { get; set; }
    }
}
