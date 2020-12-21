namespace Fanex.NAP.CTS.Services
{
    using System.Collections.Generic;

    public interface ICustomerClassifyService
    {
        bool RescanCustomerClassification(List<int> customers);
    }
}
