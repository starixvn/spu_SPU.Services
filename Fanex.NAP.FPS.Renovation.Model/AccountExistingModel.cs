using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Fanex.NAP.FPS.Renovation.Model
{
    public class AccountExistedModel : ExistingAccountModel
    {
        public string LoginNameWithPrefix { get; set; }

        public int SubscriberID { get; set; }

        public int AccountID { get; set; }

        public bool IsExisted { get; set; }

        public int IsDeposit { get; set; }

        public DateTime CreatedDate { get; set; }
    }
}
