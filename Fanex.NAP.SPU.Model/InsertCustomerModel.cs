namespace Fanex.NAP.SPU.Model
{
    public class InsertCustomerModel
    {
        public int CTSCustID { get; set; }

        public int CustID { get; set; }

        public int CustSubID { get; set; }

        public bool IsExisting { get; set; }

        public int SubscriberID { get; set; }

        public string UserName { get; set; }

        public string RegisterName { get; set; }
    }
}
