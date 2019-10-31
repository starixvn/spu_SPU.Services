namespace Fanex.NAP.FPS.Renovation.RawTransaction.Service
{
    using Fanex.NAP.FPS.Renovation.Model;
    using System.Collections.Generic;

    interface ICapturingDataService
    {     
        int InsertTransaction(List<RawTransactionModel> rawTrans);

        DeviceInfoModel GetDeviceInfo(string deviceCode);
    }
}
