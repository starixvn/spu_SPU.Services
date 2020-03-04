using Fanex.NAP.DCS.Model;
using Fanex.NAP.SPU.Model;
using System.Collections.Generic;

namespace Fanex.NAP.DCS.Device.Service.Services.Interface
{
    public interface IDataCenterService
    {      
        DataResult InsertDevice(List<DevicePackage> transId);
    }
}
