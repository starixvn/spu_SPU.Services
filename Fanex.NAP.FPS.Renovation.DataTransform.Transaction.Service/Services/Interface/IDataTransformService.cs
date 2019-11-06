using Fanex.NAP.FPS.Renovation.Model;
using System.Threading.Tasks;

namespace Fanex.NAP.FPS.Renovation.DataTransform.Transaction.Service.Services.Interface
{
    interface IDataTransformService
    {
        IDataTransformService Initial(int fromId, int toId);
               
        IDataTransformService UserAgentProcessing();

        IDataTransformService TransactionProcessing();

        IDataTransformService UnhandleSubscriberAccountProcessing();
    }
}
