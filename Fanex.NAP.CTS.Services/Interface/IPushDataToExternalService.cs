namespace Fanex.NAP.CTS.Services
{
    using Fanex.NAP.CTS.Model;
    using System.Collections.Generic;

    public interface IPushDataToExternalService
    {
        bool PushCustomerClassificationToMain(IEnumerable<LatestClassificationModel> customerClassifications, int retryTime);

        void PushCustomerClassificationToSoon88(IEnumerable<LatestClassificationModel> customerClassifications);
    }
}
