using System;

namespace Fanex.NAP.FPS.Renovation.Model
{
    public class CustomerInfoModel
    {
        private int custId;

        public int AccountId { get; set; }

        public int SubscriberId { get; set; }

        public int CustID
        {
            get { return SubAccId != 0 ? 0 - SubAccId : custId;  }
            set { custId = value; }
        }

        public int SubAccId { get; set; }        

        public string Username { get; set; }

        public string Username2 { get; set; }

        public int RoleID { get; set; }

        public string Site { get; set; }

        public int IsDeposit { get; set; }

        public int CurrencyId { get; set; }

        public string CurrencyName { get; set; }

        public DateTime CreatedDate { get; set; }

        public string LoginName { get; set; }

        public string LoginNameWithPrefix { get; set; }

        public string SubscriberName { get; set; }
    }
}
