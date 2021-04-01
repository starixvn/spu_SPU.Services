namespace Fanex.NAP.CTS.Services
{
    using Fanex.NAP.CTS.Model;
    using System.Collections.Generic;
    using static Fanex.NAP.SPU.Core.Constants;

    public interface IPushDataToExternalService
    {
        bool PushCustomerClassificationToMain(IEnumerable<LatestClassificationModel> customerClassifications, int retryTime);

        void PushCustomerClassificationToSoon88(IEnumerable<LatestClassificationModel> customerClassifications);

        bool UpdateDangerLevelToMain(IEnumerable<DangerLevelModel> dangerLevels, DANGERLEVEL typeId, int retryTime);
    }
}
