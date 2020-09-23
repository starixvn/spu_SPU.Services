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

        public int TargetCategoryId { get; set; }

        public bool Is60Day { get; set; }

        public bool IsPassed { get; set; }
    }
}
