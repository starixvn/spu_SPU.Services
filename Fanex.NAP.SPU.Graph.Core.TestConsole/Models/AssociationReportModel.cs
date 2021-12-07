using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fanex.NAP.SPU.Graph.Core.TestConsole.Models
{
    public class AssociationReportModel
    {
        public long CTSCustID { get; set; }

        public string Username { get; set; }

        public string AccountStatus { get; set; }

        public List<string> EvidenceCodes { get; set; }

        public List<string> Categories { get; set; }

        public string SubscriberName { get; set; }
    }
}
