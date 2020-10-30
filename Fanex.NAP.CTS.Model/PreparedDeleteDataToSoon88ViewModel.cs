using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fanex.NAP.CTS.Model
{
    public class PreparedDeleteDataToSoon88ViewModel
    {
        public string action
        {
            get { return "unblock"; }
        }

        public string reason { get; set; }

        public string account { get; set; }
    }
}
