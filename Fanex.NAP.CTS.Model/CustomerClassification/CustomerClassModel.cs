namespace Fanex.NAP.CTS.Model
{
    using Fanex.NAP.FS.XML;

    [XName("r")]
    public class CustomerClassModel
    {
        [XName("id")]
        public long CustID { get; set; }

        [XName("c")]
        public int CustomerClass { get; set; }

    }
}
