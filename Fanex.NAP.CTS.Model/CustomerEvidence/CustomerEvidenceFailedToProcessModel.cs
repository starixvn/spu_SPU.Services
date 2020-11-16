namespace Fanex.NAP.CTS.Model
{
    public class CustomerEvidenceFailedToProcessModel
    {
        public long ID { get; set; }

        public int CTSCustID { get; set; }
       
        public int SubscriberID { get; set; }

        public int EvidenceID { get; set; }

        public string ErrorContent { get; set; }

    }
}
