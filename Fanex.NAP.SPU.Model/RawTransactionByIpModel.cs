namespace Fanex.NAP.SPU.Model
{
    using Newtonsoft.Json;
    using System;
    using System.Numerics;

    public class RawTransactionByIpModel
    {
        private DateTime _transTime;               

        [JsonIgnore]
        public sbyte BatchId { get; set; }

        public long TransId { get; set; }

        public int AccountId { get; set; }

        public int SubscriberId { get; set; }

        public string Ip { get; set; }

        public decimal IpId { get; set; }

        public string TransTime
        {
            get
            {
                return _transTime.ToString("yyyy-MM-dd hh:mm:ss.fff");
            }
            set
            {
                _transTime = Convert.ToDateTime(value);
            }
        }
    }
}
