using FileHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExportFPSData
{
    public class FPSDataModel
    {
        public int TransId { get; set; }

        public long SubTransId { get; set; }

        public string LoginName { get; set; }

        public string SubName { get; set; }

        public DateTime TransTime { get; set; }

        public DateTime CreatedDate { get; set; }

        public string DeviceCode { get; set; }

        public string IP { get; set; }

        public string ActionName { get; set; }

        public string ActionResult { get; set; }  

        public string Url { get; set; }

        public string UserAgent { get; set; }
    }
}
