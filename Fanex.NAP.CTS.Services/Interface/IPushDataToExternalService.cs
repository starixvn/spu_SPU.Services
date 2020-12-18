namespace Fanex.NAP.CTS.Services
{
    using Fanex.NAP.CTS.Model;
    using System.Collections.Generic;

    public interface IPushDataToExternalService
    {
        bool PushCustomerClassificationToExternal(IEnumerable<LatestClassificationModel> customerClassifications);

    }
}
