namespace Fanex.NAP.FPS.Renovation.Model
{
    public class DevicePackage
    {
        public long TransId { get; set; }

        public long AccountId { get; set; }

        public long SubscriberId { get; set; }

        public string UserAgentKey { get; set; }

        public string DeviceCode { get; set; }

        public int BatchId { get; set; }
    }
}
