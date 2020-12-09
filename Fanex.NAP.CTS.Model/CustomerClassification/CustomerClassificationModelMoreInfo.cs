namespace Fanex.NAP.CTS.Model
{
    using System;
    using System.Xml.Serialization;

    public class CustomerClassificationModelMoreInfo
    {
        public long CustId { get; set; }

        public int SportGroupId { get; set; }

        public int CategoryId { get; set; }

        public DateTime CreatedTime { get; set; }

        public decimal TurnoverRM { get; set; }

        public decimal WinlossRM { get; set; }

        public Int64 BetCount { get; set; }

        public int ActiveDays { get; set; }

        public bool IsGeneralCategory { get; set; }
    }
}