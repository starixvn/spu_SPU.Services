using Newtonsoft.Json;

namespace Fanex.NAP.FPS.Renovation.Model
{
    public class RawAccountModel
    {
        [JsonIgnore]
        public int BatchId { get; set; }

        public int AccountId { get; set; }

        public string LoginNameWithPrefix { get; set; }

        public int SubscriberId { get; set; }
    }
}
