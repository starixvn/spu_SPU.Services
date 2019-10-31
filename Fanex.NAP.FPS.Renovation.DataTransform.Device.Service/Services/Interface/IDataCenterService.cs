using Fanex.NAP.FPS.Renovation.Model;
using System.Collections.Generic;

namespace Fanex.NAP.FPS.Renovation.DataTransform.Device.Service.Services.Interface
{
    public interface IDataCenterService
    {      
        DataResult InsertDevice(List<DevicePackage> transId);
    }
}
