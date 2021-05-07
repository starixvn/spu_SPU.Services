using Fanex.NAP.DCS.Model;
using System.Threading.Tasks;

namespace Fanex.NAP.DCS.Transaction.Service.Services.Interface
{
    interface IDataTransformService
    {
        IDataTransformService Initial(long fromId, long toId);
               
        IDataTransformService UserAgentProcessing();

        IDataTransformService TransactionProcessing();

        IDataTransformService UnhandleSubscriberAccountProcessing();
    }
}
