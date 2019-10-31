using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fanex.NAP.FPS.Renovation.Model
{
   public class AccountModel
    {
        public int AccountID { get; set; }

        public int CustID { get; set; }

        public string Username { get; set; }

        public string Username2 { get; set; }

        public int SubscriberID { get; set; }

        public int RoleID { get; set; }

        public int CurrencyID { get; set; }

        public string CurrencyName { get; set; }

        public bool IsDeposit { get; set; }

        public DateTime CreatedDate { get; set; }
    }
}
