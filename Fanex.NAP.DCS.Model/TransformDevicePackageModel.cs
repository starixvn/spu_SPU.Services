using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fanex.NAP.DCS.Model
{
   public class TransformDevicePackageModel
    {
        public List<DevicePackage> Devices { get; set; }

        public int ChunkSize { get; set; }
    }
}
