using Fanex.NAP.FPS.Renovation.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fanex.NAP.FPS.Renovation.Services.Interface
{
    public interface IDataTransformService
    {
        List<StagingRawTransactionModel> GetStagingRawTransaction(int isProcessed, int numberOfTicket, int numberOfBatch);
    }
}
