using FileHelpers;
using System;
using System.Numerics;

namespace Fanex.NAP.DCS.Model
{
    [DelimitedRecord("|")]
    public class RawTransactionModel
    {
        public string LoginName { get; set; }

        public string SubscriberName { get; set; }

        [FieldConverter(ConverterKind.Date, "yyyy-MM-dd HH:mm:ss.ffff")]
        public DateTime TransTime { get; set; }

        [FieldConverter(ConverterKind.Date, "yyyy-MM-dd HH:mm:ss.ffff")]
        public DateTime CreatedDate { get; set; }

        public string DeviceCode { get; set; }

        public string FingerprintCode { get; set; }

        public string FingerprintMoreInfo { get; set; }

        public string UserAgent { get; set; }
                
        public string IP { get; set; }

        [FieldConverter(typeof(ConvertBigInteger))]
        public BigInteger IPId { get; set; }

        public int PluginID { get; set; }

        public string URL { get; set; }

        public string Action { get; set; }

        public string ActionResult { get; set; }

        public string InvalidDevice { get; set; }

        public int TransStatus { get; set; }

        [FieldHidden]
        public int TransId { get; set; }
    }

    public class ConvertBigInteger : ConverterBase
    {
        public override object StringToField(string from)
        {
            return Convert.ToDecimal(from);
        }
    }
}
