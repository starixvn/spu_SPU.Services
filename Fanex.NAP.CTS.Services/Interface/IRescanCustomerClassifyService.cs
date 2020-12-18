namespace Fanex.NAP.CTS.Services
{
    using Fanex.NAP.CTS.Model;
    using System.Collections.Generic;

    public interface IRescanCustomerClassificationService
    {
        bool RescanCustomerClassification(List<TypeCategoryCustomerModel> customers);

        bool PushCustomerClassificationToExternal(IEnumerable<TypeCategoryCustomerModel> typeCategoryCusts);
    }
}
