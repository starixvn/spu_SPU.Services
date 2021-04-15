namespace Fanex.NAP.CTS.Services
{
    using System.Collections.Generic;
    using static Fanex.NAP.SPU.Core.Constants;

    public interface ICustomerClassifyService
    {
        void RescanCustomerClassify(List<int> customers, bool isCheckPAFromNAP, bool isPushCCToSoon88, FLOWTYPERESCANCC flowTypeRescanCC);
    }
}
