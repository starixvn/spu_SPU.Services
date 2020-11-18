namespace Fanex.NAP.CTS.Model
{
    using System;

    public class CustomerEvidenceFromFileModel
    {
        public long ID { get; set; }

        public long CTSCustID { get; set; }

        public int SubscriberID { get; set; }

        public int CustID { get; set; }

        public int EvidenceID { get; set; }

        public string Remark { get; set; }

        public bool IsOverwrite { get; set; }

        public string FileKey { get; set; }

        public DateTime CreatedDate { get; set; }

    }
}
