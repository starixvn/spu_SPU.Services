using Newtonsoft.Json;
using System;
using System.Xml.Serialization;

namespace Fanex.NAP.FPS.Renovation.Model
{    
    public class UserAgentModel
    {
        [JsonIgnore]
        public string UserAgent { get; set; }

        [JsonIgnore]
        public int OSId { get; set; }

        [JsonIgnore]
        public int BrowserId { get; set; }
 
        public string OSName { get; set; }
 
        public string BrowserName { get; set; }

        public long UserAgentId { get; set; }

        public DateTime CreatedDate { get; set; }
    }
}
