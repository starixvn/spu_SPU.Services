using Newtonsoft.Json;
using System;
using System.Xml.Serialization;

namespace Fanex.NAP.DCS.Model
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

        public string UserAgentKey { get; set; }

        public DateTime CreatedDate { get; set; }
    }
}
