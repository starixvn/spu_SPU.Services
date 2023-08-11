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

        public int SubscriberId { get; set; }

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

        [FieldHidden]
        public bool IsIPV6 { get; set; }

        public int PluginID { get; set; }

        public string URL { get; set; }

        public string Action { get; set; }

        public string ActionResult { get; set; }

        public string InvalidDevice { get; set; }

        public int TransStatus { get; set; }

        [FieldHidden]
        public int TransId { get; set; }

        [FieldConverter(typeof(ConvertBigInteger))]
        public BigInteger FPSTransId { get; set; }

        public string TransKey { get; set; }

        public string FlaggedCode { get; set; }

        public string BotDetection { get; set; }

        public string BotComponent { get; set; }

        public string Country { get; set; }

        public string CountryCode { get; set; }

        public string Region { get; set; }

        public string City { get; set; }

        public string ISP { get; set; }

        public long IPInfoCode { get; set; }
    }

    public class ConvertBigInteger : ConverterBase
    {
        public override object StringToField(string from)
        {
            return from.ToString();
        }
    }
}
