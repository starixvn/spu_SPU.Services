using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fanex.NAP.FPS.Renovation.Model
{
    public class AssociationByCustomerModel
    {
        public int FromCustId { get; set; }

        public int ToCustId { get; set; }

        public string ToOWUsername { get; set; }

        public string CurrencyName { get; set; }
    }
}
