namespace Fanex.NAP.CTS.Model
{
    public class CustomerEvidenceFailedToProcessModel
    {
        public string FileKey { get; set; }

        public long CTSCustID { get; set; }
       
        public int SubscriberID { get; set; }

        public int EvidenceID { get; set; }

        public string ErrorContent { get; set; }

    }
}
