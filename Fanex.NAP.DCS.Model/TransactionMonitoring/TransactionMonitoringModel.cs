namespace Fanex.NAP.DCS.Model.TransactionMonitoring
{
    using System;   

    public class TransactionMonitoringModel
    {
        public int SubscriberID { get; set; }

        public DateTime TransDate { get; set; }

        public int TransTotal { get; set; }
    }
}
