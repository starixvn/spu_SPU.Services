namespace Fanex.NAP.CTS.Model
{
    using System;
    using System.Xml.Serialization;

    public class CustomerClassificationModelMoreInfo : CustomerClassificationModel
    {
        public decimal TurnoverRM { get; set; }

        public decimal WinlossRM { get; set; }

        public Int64 BetCount { get; set; }

        public int ActiveDays { get; set; }
    }
}