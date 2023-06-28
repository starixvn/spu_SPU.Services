namespace Fanex.NAP.DCS.RawTransaction.Service
{
    using Fanex.NAP.DCS.Model;
    using System.Collections.Generic;

    interface ICapturingDataService
    {     
        int InsertTransaction(List<RawTransactionModel> rawTrans, bool isTransOfDBExtra);

        DeviceInfoModel GetDeviceInfo(string deviceCode);
    }
}
