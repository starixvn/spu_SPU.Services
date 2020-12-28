namespace Fanex.NAP.CTS.Services
{
    using System.Collections.Generic;

    public interface ICustomerClassifyService
    {
        void RescanCustomerClassification(List<int> customers);
    }
}
