using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fanex.NAP.SPU.Model
{
   public class RawAssociationModel
    {
        private DateTime createdTime;

        [JsonIgnore]
        public int BatchId { get; set; }

        public int AssociationId { get; set; }

        public int AccountId { get; set; }

        public int DeviceId { get; set; }

        public string CreatedTime { 
            get
            {
                return createdTime.ToString("yyyy-MM-dd hh:mm:ss.fff");
            }
            set
            {
                createdTime = Convert.ToDateTime(value);
            }
        }

        public int SubscriberId { get; set; }
    }
}
