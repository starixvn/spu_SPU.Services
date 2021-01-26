namespace Fanex.NAP.SPU.Model
{
    using System;

    public class CustomerMainDbModel
    {
        private DateTime createdDate;

        public int BatchID { get; set; }

        public int CustID { get; set; }

        public int CustSubID { get; set; }

        public string UserName { get; set; }

        public string UserName2 { get; set; }

        public string Site { get; set; }

        public int SiteID { get; set; }

        public int RoleID { get; set; }

        public int CurrencyID { get; set; }

        public string Currency { get; set; }

        public int SRecommend { get; set; }

        public int MRecommend { get; set; }

        public int Recommend { get; set; }

        public DateTime CreatedDate
        {
            get { return Convert.ToDateTime(createdDate.ToString("yyyy-MM-dd HH:mm:ss.ffff")); }
            set { createdDate = value; }
        }

        public int CustStatusID { get; set; }

        public int Danger1 { get; set; }

        public int Danger2 { get; set; }

        public int Danger3 { get; set; }
    }
}
