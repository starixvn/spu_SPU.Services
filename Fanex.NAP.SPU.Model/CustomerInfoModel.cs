namespace Fanex.NAP.SPU.Model
{
    using Newtonsoft.Json;
    using System;

    public class CustomerInfoModel
    {
        private DateTime createdDate;

        public int CustID { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string UserName { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string UserName2 { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Site { get; set; }

        public int SiteID { get; set; }

        public int RoleID { get; set; }

        public int CurrencyID { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Currency { get; set; }

        public int SRecommend { get; set; }

        public int MRecommend { get; set; }

        public int Recommend { get; set; }

        public DateTime CreatedDate
        {
            get { return Convert.ToDateTime(createdDate.ToString("yyyy-MM-dd HH:mm:ss.ffff")); }
            set { createdDate = value; }
        }

        public int UpdateType { get; set; }
    }
}
