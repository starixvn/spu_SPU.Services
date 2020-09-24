using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fanex.NAP.CTS.Model
{
    public class ProbationModel
    {
        public long CustId { get; set; }

        public int SportGroupId { get; set; }

        public int CategoryId { get; set; }

        public bool IsPassed { get; set; }
    }
}
