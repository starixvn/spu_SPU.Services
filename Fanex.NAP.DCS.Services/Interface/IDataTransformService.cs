using Fanex.NAP.DCS.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fanex.NAP.DCS.Services.Interface
{
    public interface IDataTransformService
    {
        List<StagingRawTransactionModel> GetStagingRawTransaction(int isProcessed, int numberOfTicket, int numberOfBatch);
    }
}
