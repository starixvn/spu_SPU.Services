using Fanex.NAP.DCS.Model;
using System.Threading.Tasks;

namespace Fanex.NAP.DCS.DataTransform.Transaction.Service.Services.Interface
{
    interface IDataTransformService
    {
        IDataTransformService Initial(int fromId, int toId);
               
        IDataTransformService UserAgentProcessing();

        IDataTransformService TransactionProcessing();

        IDataTransformService UnhandleSubscriberAccountProcessing();
    }
}
