namespace Fanex.NAP.CTS.Services
{
    using System.Collections.Generic;

    public interface ICustomerClassifyService
    {
        void RescanCustomerClassify(List<int> customers, bool isRescanPA, bool isPushCCToSoon88);
    }
}
