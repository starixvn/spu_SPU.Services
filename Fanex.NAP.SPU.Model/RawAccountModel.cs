using Newtonsoft.Json;

namespace Fanex.NAP.SPU.Model
{
    public class RawAccountModel
    {
        [JsonIgnore]
        public int BatchId { get; set; }

        public int AccountId { get; set; }                

        public string LoginName { get; set; }

        public int SubscriberId { get; set; }
    }
}
