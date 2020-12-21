namespace Fanex.NAP.CTS.Services
{
    using Fanex.NAP.CTS.Model;
    using System.Collections.Generic;

    public interface IRescanCustomerClassifyService
    {
        bool RescanCustomerClassification(List<int> customers);
    }
}
