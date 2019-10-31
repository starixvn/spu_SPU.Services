namespace Fanex.NAP.FPS.Renovation.DataTransform.Device.Service.Services.Implement
{
    using Fanex.NAP.FPS.Renovation.DataAccess;
    using Fanex.NAP.FPS.Renovation.DataTransform.Device.Service.Services.Interface;
    using Fanex.NAP.FPS.Renovation.Model;
    using Newtonsoft.Json;
    using System.Collections.Generic;

    public class DataCenterService : IDataCenterService
    {
        private readonly DataAccess.DataCenterDataAccess dataCenter;
        
        public DataCenterService()
        {
            dataCenter = new DataCenterDataAccess();
        }

        public DataResult InsertDevice(List<DevicePackage> transId)
        {
            var transJson = JsonConvert.SerializeObject(transId);

            return dataCenter.InsertDevice(transJson);
        }
    }
}
