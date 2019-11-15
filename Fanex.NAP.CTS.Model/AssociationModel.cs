using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fanex.NAP.CTS.Model
{
   public class AssociationModel
    {
        public int CustId { get; set; }

        public int DeviceId { get; set; }

        public int SubscriberId { get; set; }

        public DateTime CreatedTime { get; set; }
    }
}
