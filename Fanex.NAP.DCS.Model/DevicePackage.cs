using System;

namespace Fanex.NAP.DCS.Model
{
    public class DevicePackage
    {
        private DateTime _transTime;
        private DateTime _createdDate;

        public long TransId { get; set; }

        public long AccountId { get; set; }

        public long SubscriberId { get; set; }

        public string UserAgentKey { get; set; }

        public string DeviceCode { get; set; }

        public string FingerprintCode { get; set; }

        public string FingerprintMoreInfo { get; set; }

        public DateTime TransTime
        {
            get
            {
                return Convert.ToDateTime(_transTime.ToString("yyyy-MM-dd HH:mm:ss.ffff"));
            }
            set
            {
                _transTime = value;
            }
        }

        public DateTime CreatedDate {
            get
            {
                return Convert.ToDateTime(_createdDate.ToString("yyyy-MM-dd HH:mm:ss.ffff"));
            }
            set
            {
                _createdDate = value;
            }
        }

        public int BatchId { get; set; }
    }
}
