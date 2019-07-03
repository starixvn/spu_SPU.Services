namespace Fanex.NAP.FS.Model
{
    using Fanex.NAP.FS.XML;

    [XName("Association")]
    public class ArbitrageCustomer
    {
        public int CustId { get; set; }

        public int GroupId { get; set; }
    }
}