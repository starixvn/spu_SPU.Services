using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fanex.NAP.SPU.Graph.Core.TestConsole.Models
{
    class GraphAssociationReportPaging
    {
        public IEnumerable<AssociationReportModel> AssociationList { get; set; }

        public int TotalItems { get; set; }
    }
}
