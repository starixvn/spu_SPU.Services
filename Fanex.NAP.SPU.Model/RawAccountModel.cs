using Newtonsoft.Json;
using System;

namespace Fanex.NAP.SPU.Model
{
    public class RawAccountModel
    {
        private DateTime lastLoginTime;

        [JsonIgnore]
        public int BatchId { get; set; }

        public int AccountId { get; set; }                

        public string LoginName { get; set; }

        public int SubscriberId { get; set; }

        public DateTime LastLoginTime
        {
            get
            {
                return Convert.ToDateTime(lastLoginTime.ToString("yyyy-MM-dd HH:mm:ss.ffff"));
            }

            set
            {
                lastLoginTime = value;
            }
        }
    }
}
