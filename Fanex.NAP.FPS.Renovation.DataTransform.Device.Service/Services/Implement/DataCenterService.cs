namespace Fanex.NAP.DCS.DataTransform.Device.Service.Services.Implement
{
    using Fanex.NAP.DCS.DataAccess;
    using Fanex.NAP.DCS.DataTransform.Device.Service.Services.Interface;
    using Fanex.NAP.DCS.Model;
    using Fanex.NAP.SPU.Model;
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
