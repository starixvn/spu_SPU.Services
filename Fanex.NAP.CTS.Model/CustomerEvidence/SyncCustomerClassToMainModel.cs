namespace Fanex.NAP.CTS.Model
{
    using Fanex.NAP.FS.XML;

    [XName("r")]
    public class SyncCustomerClassToMainModel
    {
        [XName("id")]
        public int CustID { get; set; }

        [XName("c")]
        public int CategoryIdMapping { get; set; }

    }
}
